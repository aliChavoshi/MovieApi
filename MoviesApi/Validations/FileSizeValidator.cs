using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MoviesApi.Validations
{
    public class FileSizeValidator : ValidationAttribute
    {
        private readonly int _maxFileSizeInMbs;
        //هرکس تورو صدا زد باید بهت حجم مورد نظرش را بده
        public FileSizeValidator(int maxFileSizeInMbs)
        {
            _maxFileSizeInMbs = maxFileSizeInMbs;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            //جنسش را مشخص میکنم که از جنس چی هست
            if (!(value is IFormFile formFile))
            {
                return ValidationResult.Success;
            }
            //اگر حجمش بیشتر بود بهش خطا بده
            return formFile.Length > _maxFileSizeInMbs * 1024 * 1024 ? new ValidationResult($"Max Size Is : {_maxFileSizeInMbs}") : ValidationResult.Success;
        }
    }
}