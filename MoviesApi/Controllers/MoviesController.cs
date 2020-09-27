using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using MoviesApi.Services;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private const string ContainerName = "Movies";
        private readonly ILogger<Movie> _logger;

        public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService, ILogger<Movie> logger)
        {
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }


        // GET: api/Movies
        [HttpGet]
        public async Task<IActionResult> GetMovies([FromQuery] PaginationDto pagination)
        {
            var top = 6;
            var today = DateTime.Today;
            var upcomingRelease = await _context.Movies.Where(x => x.ReleaseDate < today).OrderBy(x => x.ReleaseDate)
                .Take(top).ToListAsync();

            var inTheaters = await _context.Movies.Where(x => x.InTheaters).Take(top).ToListAsync();

            var result = new IndexMoviePageDto()
            {
                InTheaters = _mapper.Map<List<MovieDto>>(inTheaters),
                UpcomingRelease = _mapper.Map<List<MovieDto>>(upcomingRelease)
            };

            return Ok(result);

            /*//IQueryable Movie
            IQueryable<Movie> queryable = _context.Movies;

            //Save Count Page To Header
            await HttpContext.InsertPaginationParametersInResponse(queryable: queryable, pagination.RecordsPerPage);
            //Insert Take Skip
            var movies = await queryable.Paginate(pagination).ToListAsync();
            //Mapping
            var moviesDtos = _mapper.Map<List<MovieDto>>(source: movies);
            //Return
            return Ok(moviesDtos);*/
        }

        /// <summary>
        /// Get Movie By Id
        /// </summary>
        /// <param name="id">Movie Id</param>
        /// <returns></returns>
        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var movieDto = _mapper.Map<MovieDetailsDto>(movie);

            return Ok(movieDto);
        }


        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] FilterMoviesDto filterMoviesDto)
        {
            IQueryable<Movie> queryable = _context.Movies;

            if (!string.IsNullOrEmpty(filterMoviesDto.Title))
            {
                queryable = queryable.Where(x => x.Title.Contains(filterMoviesDto.Title));
            }

            if (filterMoviesDto.InTheaters)
            {
                queryable = queryable.Where(x => x.InTheaters);
            }

            if (filterMoviesDto.UpcomingReleases)
            {
                var today = DateTime.Today;
                queryable = queryable.Where(x => x.ReleaseDate < today);
            }

            if (filterMoviesDto.GenreId != 0)
            {
                queryable = queryable.Where(
                    x => x.MoviesGenres.Select(c => c.GenreId).Contains(filterMoviesDto.GenreId));
            }

            if (!string.IsNullOrEmpty(filterMoviesDto.OrderingField))
            {
                try
                {
                    queryable = queryable.OrderBy($"{filterMoviesDto.OrderingField} {(filterMoviesDto.AscendingOrder ? "ascending" : "descending")}");
                }
                catch
                {
                    _logger.LogWarning("Could not order by field : " + filterMoviesDto.OrderingField);
                }
            }

            await HttpContext.InsertPaginationParametersInResponse(queryable, filterMoviesDto.RecorsPerPage);

            var movies = await queryable.Paginate(filterMoviesDto.Pagination).ToListAsync();


            return Ok(_mapper.Map<List<MovieDto>>(movies));
        }


        // POST: api/Movies
        [HttpPost]
        public async Task<IActionResult> PostMovie([FromForm] MovieCreationDtos movieCreationDtos)
        {
            //New Movie
            //نباید آیدی در این بخش دست کاربر باشه
            var movie = _mapper.Map<Movie>(source: movieCreationDtos);

            #region SaveFile

            //این کار بعد از mapper  انجام شد   چون   در  mapper  بهش گفتم که ignore  کن من خودم داخلش میریزم.
            if (movieCreationDtos.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDtos.Poster.CopyToAsync(memoryStream);
                    //content
                    var content = memoryStream.ToArray();
                    //extension
                    var extension = Path.GetExtension(movieCreationDtos.Poster.FileName);
                    /*var extension = personCreationDtOs.Picture.FileName.Split(".")[1];*/
                    movie.Poster = await _fileStorageService
                        .SaveFile(content: content, extension: extension, containerName: ContainerName, contentType: movieCreationDtos.Poster.ContentType);
                }
            }

            #endregion

            //For Order
            AnnotateActorsOrder(movie);

            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();


            //چون get  جنسش فرض نیکند باید عوض کنم
            var movieDto = _mapper.Map<MovieDto>(movie);

            return CreatedAtAction("GetMovie", new { id = movieDto.Id }, movieDto);
        }


        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie([FromRoute] int id, [FromForm] MovieCreationDtos movieCreationDtos)
        {
            var movieDb = await _context.Movies.FindAsync(id);
            if (movieDb == null)
            {
                return NotFound();
            }
            //از چپی میخواهم بریزم توی راستی 
            //نمیخواهم مدل جدید درست کنم
            movieDb = _mapper.Map(source: movieCreationDtos, destination: movieDb);

            #region EditFile

            //این کار بعد از mapper  انجام شد   چون   در  mapper  بهش گفتم که ignore  کن من خودم داخلش میریزم.
            if (movieCreationDtos.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDtos.Poster.CopyToAsync(memoryStream);
                    //content
                    var content = memoryStream.ToArray();
                    //extension
                    var extension = Path.GetExtension(movieCreationDtos.Poster.FileName);
                    /*var extension = personCreationDtOs.Picture.FileName.Split(".")[1];*/
                    movieDb.Poster = await _fileStorageService
                        .EditFile(content: content, extension: extension, containerName: ContainerName, fileRoute: movieDb.Poster, contentType: movieCreationDtos.Poster.ContentType);
                }
            }

            #endregion

            //Delete Actors and Genres From Movie By Query Sql
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"delete from MoviesActors where MovieId={movieDb.Id}; delete from MoviesGenres where MovieId={movieDb.Id}");
            //Order
            AnnotateActorsOrder(movieDb);

            _context.Entry(movieDb).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var movieDto = _mapper.Map<MovieDto>(movieDb);

            return CreatedAtAction("GetMovie", new { id = movieDto.Id }, movieDto);
        }


        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchMovie(int id, [FromBody] JsonPatchDocument<MoviePatchDtos> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            //Get Data From DB
            Movie entityFromDb = await _context.Movies.FindAsync(id);
            if (entityFromDb == null)
            {
                return NotFound();
            }

            //ابتدا مدل دیتا بیس را با مدل پچ مپ میکنم و مقادیر دیتا بیس را داخل یک کتغیر میریزم
            //به این دلیل این کار را کردم که بتوانم مدل دیتابیس را به مدل ورودی نزدیک کنم
            var entityDto = _mapper.Map<MoviePatchDtos>(source: entityFromDb);

            //حالا مدل ورودی را به مدل دیتا بیس باید اپلای کنم
            patchDocument.ApplyTo(entityDto, modelState: ModelState);
            //الان همه چیز داخل entityDTO  میباشد 
            //شاید کاربر مقادیر نال ارسال کرده باشه 
            var isValid = TryValidateModel(entityDto);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            //حالا باید مدلی که ویرایش کردیم را به مدل دیتابیس ارسال کنیم تا آپدیت بشه
            _mapper.Map(source: entityDto, destination: entityFromDb);

            //Save
            _context.Update(entityFromDb);
            await _context.SaveChangesAsync();
            return Ok(entityFromDb);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            //Delete Actors and Genres From Movie By Query Sql
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"delete from MoviesActors where MovieId={id}; delete from MoviesGenres where MovieId={id}");

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return Ok(movie);
        }


        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors.Any())
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }
    }
}
