using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesApi.DTOs
{
    public class PersonDtos
    {
        public int Id { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [MaxLength(500)]
        public string Name { get; set; }

        [Display(Name = "بیوگرافی")]
        [MaxLength(500)]
        public string Biography { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "عکس کاربر")]
        public string Picture { get; set; }
    }
}