using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Entities
{
    public class Person : BaseEntity
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

        [Display(Name = "عکس کاربر")]
        public string Picture { get; set; }

        public List<MoviesActors> MoviesActors { get; set; }

    }
}