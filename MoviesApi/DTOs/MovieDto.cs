using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesApi.DTOs
{
    public class MovieDto
    {

        public int Id { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool InTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster { get; set; }

    }
}