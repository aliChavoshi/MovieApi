using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Validations
{
    public class FirstLetterUppercaseAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            var firstLetter = value.ToString()?[0].ToString();
            if (firstLetter != null && firstLetter != firstLetter.ToUpper())
            {
                return new ValidationResult("حرف اول باید بزرگ باشد");
            }
            return ValidationResult.Success;
        }
    }
}