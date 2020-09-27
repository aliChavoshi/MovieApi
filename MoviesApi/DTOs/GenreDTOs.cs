using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.DTOs
{
    public class GenreDtOs : IGenerateHateoasLinks
    {
        public int Id { get; set; }

        [Display(Name = "نام")]
        public string Name { get; set; }

        public List<Link> Links { get; set; } = new List<Link>();


        public void GenerateLinks(IUrlHelper urlHelper)
        {
            Links.Add(new Link(href: urlHelper.Link("GetGenre", new { id = Id }), rel: "Get-Genre", method: "GET"));
            Links.Add(new Link(href: urlHelper.Link("PutGenre", new { id = Id }), rel: "Put-Genre", method: "PUT"));
            Links.Add(new Link(href: urlHelper.Link("DeleteGenre", new { id = Id }), rel: "Delete-Genre", method: "DELETE"));
        }

        public ResourceCollection<GenreDtOs> GenerateLinksCollection<GenreDtOs>(List<GenreDtOs> dtos, IUrlHelper urlHelper)
        {
            //ابتدا مقدار های قبلی که در بالا ساخته شده است را بهش دادم
            //Values
            var resourceCollection = new ResourceCollection<GenreDtOs>(dtos);
            //Create Links
            resourceCollection.Links.Add(new Link(href: urlHelper.Link("CreateGenre", new { }), rel: "Create-Genre", method: "POST"));
            resourceCollection.Links.Add(new Link(href: urlHelper.Link("GetGenres", new { }), rel: "Get-Genre", method: "GET"));

            return resourceCollection;
        }
    }
}
