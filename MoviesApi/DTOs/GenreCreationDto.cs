using System.ComponentModel.DataAnnotations;

namespace MoviesApi.DTOs
{
    public class GenreCreationDto
    {

        [Display(Name = "نام")]
        public string Name { get; set; }
    }
}