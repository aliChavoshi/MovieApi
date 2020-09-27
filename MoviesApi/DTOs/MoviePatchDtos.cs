using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MoviesApi.Validations;

namespace MoviesApi.DTOs
{
    public class MoviePatchDtos
    {
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500)]
        public string Title { get; set; }

        public string Summary { get; set; }

        public bool InTheaters { get; set; }

        public DateTime ReleaseDate { get; set; }

       
    }
}