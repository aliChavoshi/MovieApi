using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.DTOs
{
    public interface IGenerateHateoasLinks
    {
        void GenerateLinks(IUrlHelper urlHelper);
        ResourceCollection<T> GenerateLinksCollection<T>(List<T> dtos, IUrlHelper urlHelper);
    }
}