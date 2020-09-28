using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.DTOs;
using MoviesApi.Helpers;

namespace MoviesApi.Controllers.V2
{
    [Route("api")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpHeaderIsPresent("X-Version","2")]
    public class RouteController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult Get()
        {
            var links = new List<Link>
            {
                //routeName :  اسم روت ها
                //self : خود همین متد
                new Link(href: Url.Link(routeName: "GetRoot", values: new { }), rel: "self", method: "Get")
            
            };

            return Ok(links);
        }
    }
}
