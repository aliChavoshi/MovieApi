using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace MoviesApi.Entities
{
    public class MovieTheater : BaseEntity
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name { get; set; }

        public Point Location { get; set; }

    }
}