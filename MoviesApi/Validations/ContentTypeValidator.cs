using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace MoviesApi.Validations
{
    public class ContentTypeValidator : ValidationAttribute
    {
        //اعتبار سنجی کلی برای همه
        private readonly string[] _validContentTypes;


        //موارد قابل قبول برای عکس این ها میباشد
        private readonly string[] _imageContentTypes = { "image/jpeg", "image/png", "image/gif" };
        //موارد قابل قبول برای فیلم
        private readonly string[] _videoContentTypes = { "video/mp4", "video/gpeg" };
        //Access Pdf
        private readonly string[] _pdfContentTypes = { "application/pdf" };

        public ContentTypeValidator(string[] validContentTypes)
        {
            _validContentTypes = validContentTypes;
        }

        //این را من بهش میگم که Image  باشه  یا Video
        public ContentTypeValidator(ContentTypeGroup contentTypeGroup)
        {
            switch (contentTypeGroup)
            {
                case ContentTypeGroup.Image:
                    _validContentTypes = _imageContentTypes;
                    break;

                case ContentTypeGroup.Video:
                    _validContentTypes = _videoContentTypes;
                    break;

                case ContentTypeGroup.Pdf:
                    _validContentTypes = _pdfContentTypes;
                    break;
            }
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
            //اگر پسوند فایل شامل موارد ذکر شده نبود بهش خطا بده
            if (!_validContentTypes.Contains(formFile.ContentType))
            {
                //Join : بخش اول میگه با چی جدا کنم
                //بخش دوم میگه لیستش رو بده به من
                return new ValidationResult(
                    $"Format File IS :  {string.Join(", ", _validContentTypes)}");
            }

            return ValidationResult.Success;
        }

        //مواردی که من میتونم انتخاب کنم
        public enum ContentTypeGroup
        {
            Image,
            Video,
            Pdf
        }
    }
}