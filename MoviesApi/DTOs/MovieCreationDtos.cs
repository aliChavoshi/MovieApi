using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Helpers;
using MoviesApi.Validations;

namespace MoviesApi.DTOs
{
    public class MovieCreationDtos : MoviePatchDtos
    {

        [FileSizeValidator(maxFileSizeInMbs: 5)]
        [ContentTypeValidator(ContentTypeValidator.ContentTypeGroup.Image)]
        public IFormFile Poster { get; set; }


        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenresIds { get; set; }


        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorCreationDto>>))]
        public List<ActorCreationDto> Actors { get; set; }
    }
}