using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MoviesApi.Entities;
using MoviesApi.Filters;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class GenresLearningController : ControllerBase
    {
        private readonly IRepository _repository;
        public GenresLearningController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet] //api/Genres
        /*[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]*/
        [ServiceFilter(typeof(MyActionFilter))]
        public IActionResult Get()
        {
            //return 200
            return Ok(_repository.GetAllGenre());
        }


        /*[HttpGet("{id:int}/{param2=ali}")] //api/genres/id/param2
        [HttpGet("Test/{id}/{param2=ali}")] //api/genres/Test/id/param2
        [HttpGet("Test")] //api/genres/Test?id=1&param2=ali
        [HttpGet("/ListGenres")] //localhost/ListGenres?id=1&param2=reza
        [HttpGet("/ListGenres/{id}/{param2}")] //localhost/ListGenres/1/ali*/
        [HttpGet("{id}")]
        public ActionResult<Genre> Get(int id, string param2 = "ali")
        {

            /*throw new ApplicationException();*/

            var model = _repository.GetById(id);
            if (model == null)
            {
                //return 404
                return NotFound();
            }
            return Ok(model);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Genre genre)
        {
            _repository.AddGenre(genre);

            return new CreatedAtActionResult("Get", "Genres", new { id = genre.Id }, genre);
            /*return Ok(genre);*/
        }

        [HttpPut]
        public IActionResult Put()
        {
            //retuen 204
            return NoContent();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            return NoContent();
        }

        /*[HttpGet("{id}")] //api/Genres/1?name=ali&test=True
        public ActionResult<string> Get(int id, string name, bool test, [FromHeader] string headerTest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }
            return id + name + headerTest;
        }*/

    }
}