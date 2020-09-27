using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private const string ContainerName = "people";

        public PeopleController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService)
        {
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        // GET: api/People
        [HttpGet(Name = "GetPeople")]
        public async Task<IActionResult> GetPerson([FromQuery] PaginationDto pagination)
        {
            //ابتدا لیست کلی را در یافت میکینیم ولی به لیست تبدیلش نمیکنیم 
            IQueryable<Person> queryble = _context.Person;

            //Insert Count Page To Header
            await HttpContext.InsertPaginationParametersInResponse(queryable: queryble, pagination.RecordsPerPage);

            //Insert Take Skip
            var persons = await queryble.Paginate(pagination).ToListAsync();

            //Create New Map List<PersonDtos>
            var personDtOs = _mapper.Map<List<PersonDtos>>(persons);

            return Ok(personDtOs);
        }



        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            var person = await _context.Person.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            var personDto = _mapper.Map<PersonDtos>(person);

            return Ok(personDto);
        }



        // POST: api/People
        [HttpPost]
        public async Task<IActionResult> PostPerson([FromForm] PersonCreationDTOs personCreationDtOs)
        {
            //مدل خالی هست من باید مدل جدید بسازم از این بخش استفاده میکنم
            var person = _mapper.Map<Person>(source: personCreationDtOs);

            #region SaveFile

            //این کار بعد از mapper  انجام شد   چون   در  mapper  بهش گفتم که ignore  کن من خودم داخلش میریزم.
            if (personCreationDtOs.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationDtOs.Picture.CopyToAsync(memoryStream);
                    //content
                    var content = memoryStream.ToArray();
                    //extension
                    var extension = Path.GetExtension(personCreationDtOs.Picture.FileName);
                    /*var extension = personCreationDtOs.Picture.FileName.Split(".")[1];*/
                    person.Picture = await _fileStorageService
                        .SaveFile(content: content, extension: extension, containerName: ContainerName, contentType: personCreationDtOs.Picture.ContentType);
                }
            }

            #endregion

            await _context.Person.AddAsync(person);
            await _context.SaveChangesAsync();

            var personDtOs = _mapper.Map<PersonDtos>(source: person);

            return CreatedAtAction("GetPerson", new { id = person.Id }, personDtOs);
        }



        // PUT: api/People/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson([FromRoute] int id, [FromForm] PersonCreationDTOs personCreationDtOs)
        {
            var personDb = await _context.Person.FindAsync(id);
            if (personDb == null)
            {
                return NotFound();
            }

            //مدل پر هست فقط میخواهم اطلاعات جدیدش را آپدیت کنم
            //مقدار picture  را در سمت AutoMapper  بهش گفتم برام  Ignore  کن واسه همین باید خودم پرش کنم
            personDb = _mapper.Map(source: personCreationDtOs, destination: personDb);

            #region EditFile

            //این کار بعد از mapper  انجام شد   چون   در  mapper  بهش گفتم که ignore  کن من خودم داخلش میریزم.
            if (personCreationDtOs.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personCreationDtOs.Picture.CopyToAsync(memoryStream);
                    //content
                    var content = memoryStream.ToArray();
                    //extension
                    var extension = Path.GetExtension(personCreationDtOs.Picture.FileName);
                    /*var extension = personCreationDtOs.Picture.FileName.Split(".")[1];*/
                    personDb.Picture = await _fileStorageService
                        .EditFile(content: content, extension: extension, containerName: ContainerName, fileRoute: personDb.Picture, contentType: personCreationDtOs.Picture.ContentType);
                }
            }

            #endregion

            _context.Entry(personDb).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(personDb);
        }



        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchPerson(int id, [FromBody] JsonPatchDocument<PersonPatchDtos> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            //Get Data From DB
            Person entityFromDb = await _context.Person.FindAsync(id);
            if (entityFromDb == null)
            {
                return NotFound();
            }

            //ابتدا مدل دیتا بیس را با مدل پچ مپ میکنم و مقادیر دیتا بیس را داخل یک کتغیر میریزم
            //به این دلیل این کار را کردم که بتوانم مدل دیتابیس را به مدل ورودی نزدیک کنم
            PersonPatchDtos entityDto = _mapper.Map<PersonPatchDtos>(source: entityFromDb);

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



        // DELETE: api/People/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson([FromRoute] int id)
        {
            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.Person.Remove(person);
            await _context.SaveChangesAsync();

            return Ok(person);
        }
    }
}
