using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.DTOs;
using MoviesApi.Helpers;

namespace MoviesApi.Controllers
{
    [Route("api")]
    [ApiController]
    [HttpHeaderIsPresent("X-Version", "1")]
    public class RouteController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult Get()
        {
            var links = new List<Link>
            {
                //routeName :  اسم روت ها
                //self : خود همین متد
                new Link(href: Url.Link(routeName: "GetRoot", values: new { }), rel: "self", method: "Get"),
                //از بقیه controller  ها اومدم گرفتم
                new Link(href: Url.Link(routeName: "CreateUser", values: new { }), rel: "Create-User", method: "POST"),
                new Link(href: Url.Link(routeName: "Login", values: new { }), rel: "Login-User", method: "POST"),
                new Link(href: Url.Link(routeName: "GetGenres", values: new { }), rel: "Get-Genres", method: "GET"),
                new Link(href: Url.Link(routeName: "GetPeople", values: new { }), rel: "Get-People", method: "GET")
            };

            return Ok(links);
        }
    }
}
