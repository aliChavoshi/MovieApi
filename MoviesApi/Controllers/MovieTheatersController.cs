using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieTheatersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public MovieTheatersController(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieTheaterDto>>> Get([FromQuery] FilterMovieTheathersDto filterMovieTheathersDto)
        {
            //هندسه
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            //lat :عرض
            //long :  طول
            var userLocation =
                geometryFactory.CreatePoint(new Coordinate(filterMovieTheathersDto.Long, filterMovieTheathersDto.Lat));

            var theaters = await _context.MovieTheaters
                //برحسب فاصله مرتب سازی کن
                .OrderBy(x => x.Location.Distance(userLocation))
                //اون هایی را بهم بده که در این فاصله هستند بر حسب کیلومتر کاربر خواهد داد
                //پارامتر اولی : میگه با چی مقایسه کنم
                //پارامتر دومی : میگه با چه معیاری مقایسه کنم
                .Where(x => x.Location.IsWithinDistance(userLocation, filterMovieTheathersDto.DistanceInKms * 1000))
                .Select(x => new MovieTheaterDto()
                {
                    Name = x.Name,
                    Id = x.Id,
                    DistanceInMeters = Math.Round(x.Location.Distance(userLocation))
                }).ToListAsync();

            return theaters;
        }
    }
}
