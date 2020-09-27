using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesApi.DTOs
{
    public class PersonPatchDtos
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "نام و نام خانوادگی")]
        [MaxLength(500)]
        public string Name { get; set; }

        [Display(Name = "بیوگرافی")]
        [MaxLength(500)]
        public string Biography { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime DateOfBirth { get; set; }
    }
}