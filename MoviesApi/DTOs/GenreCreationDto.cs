using System.ComponentModel.DataAnnotations;
using MoviesApi.Services;

namespace MoviesApi.DTOs
{
    public class GenreCreationDto 
    {

        [Display(Name = "نام")]
        public string Name { get; set; }
    }
}