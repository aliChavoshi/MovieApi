using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Filters;
using MoviesApi.Helpers;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    /*[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]*/
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GenresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }



        [HttpGet(Name = "GetGenres")] //api/Genres
        [EnableCors(PolicyName = "AllowRequestIO")]
        [ServiceFilter(typeof(GenreHateoasAttribute))]
        /*[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "admin")]*/
        public async Task<IActionResult> Get()
        {
            //Get Data From DB
            var genres = await _context.Genres.ToListAsync();
            //میخواهم لیست بالا را به داخل لیست خودم تبدلی کنم
            //< > مدل من
            //( ) مدلی که دریافت کرده ام
            var genresDtos = _mapper.Map<List<GenreDtOs>>(genres);

            return Ok(genresDtos);
        }



        [HttpGet("{id}", Name = "GetGenre")] //api/Genres/id
        [ServiceFilter(typeof(GenreHateoasAttribute))]
        public async Task<IActionResult> Get(int id)
        {
            var model = await _context.Genres.FindAsync(id);
            if (model == null)
            {
                //return 404
                return NotFound();
            }

            var genreDto = _mapper.Map<GenreDtOs>(model);


            return Ok(genreDto);
        }



        [HttpPost(Name = "CreateGenre")]
        public async Task<IActionResult> Post([FromBody] GenreCreationDto genreCreation)
        {
            //مدلی که سمت کاربر برای من ارسال میشود نباید آیدی داشته باشد
            //لذا یک مدل ساختم که بدون آیدی باشد 
            //لذا جدا این بخش را مپ کردم
            //حالا باید مدل کاربر را به سمت مدل دیتابیس مپ کنم
            var genre = _mapper.Map<Genre>(genreCreation);
            await _context.AddAsync(genre);
            await _context.SaveChangesAsync();
            //reverse map
            //در نهایت مقدار اضافه شده را باید به مدل خودم مپ کنم 
            var genreDto = _mapper.Map<GenreDtOs>(genre);

            //back to action
            //api/Genre/id
            return new CreatedAtActionResult("Get", "Genres", new { id = genreDto.Id }, genreDto);
        }



        [HttpPut("{id}", Name = "PutGenere")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] GenreCreationDto genreDto)
        {
            var genre = _mapper.Map<Genre>(genreDto);
            genre.Id = id;
            _context.Entry(genre).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            //retuen 204
            return Ok(genre);
        }




        [HttpDelete("{id}", Name = "DeleteGenre")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var model = await _context.Genres.FindAsync(id);
            if (model == null)
            {
                //return 404
                return NotFound();
            }

            _context.Entry(model).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            var genreDto = _mapper.Map<GenreDtOs>(model);
            return Ok(genreDto);
        }


    }
}