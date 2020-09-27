using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis.CSharp;
using MoviesApi.DTOs;

namespace MoviesApi.Helpers
{
    public class GenreHateoasAttribute : HateosAttribute
    {
        //تزریق وابستگی
        private readonly LinksGenerator _linksGenerator;
        public GenreHateoasAttribute(LinksGenerator linksGenerator)
        {
            _linksGenerator = linksGenerator;
        }

        //از کلاسی که ارث بری کردیم این متد که override  میباشد حاصل میشود 
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var includeHateoas = ShouldIncludeHateas(context);
            if (!includeHateoas)
            {
                //next : For Run 
                await next();
                return;
            }
            //متدهایی که در ساخت لینک ساختم در این بخش در دسترس است
            await _linksGenerator.Genereate<GenreDtOs>(context, next);
        }
    }
}