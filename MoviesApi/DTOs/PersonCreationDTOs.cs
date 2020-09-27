using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MoviesApi.Validations;

namespace MoviesApi.DTOs
{
    public class PersonCreationDTOs : PersonPatchDtos
    {

        [FileSizeValidator(4)]
        [ContentTypeValidator(ContentTypeValidator.ContentTypeGroup.Image)]
        public IFormFile Picture { get; set; }

    }
}