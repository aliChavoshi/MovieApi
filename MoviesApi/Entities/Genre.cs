using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MoviesApi.Services;
using MoviesApi.Validations;

namespace MoviesApi.Entities
{
    public class Genre : BaseEntity,IId
    {
        [Required(ErrorMessage = "لطفا مقدار {0} را وارد نمائید")]
        [Display(Name = "نام")]
        public string Name { get; set; }

        public List<MoviesGenres> MoviesGenres { get; set; }

    }




    /*public class Genre : BaseEntity, IValidatableObject
    {
        [Required(ErrorMessage = "لطفا مقدار {0} را وارد نمائید")]
        [Display(Name = "نوع")]
        /*[FirstLetterUppercase]#1#
        public string Name { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name)) yield break;

            var firstLetter = Name[0].ToString();
            if (firstLetter != firstLetter.ToUpper())
            {
                yield return new ValidationResult("حرف اول باید بزرگ باشد ", new string[] { nameof(Name) });
            }
        }
    }*/
}