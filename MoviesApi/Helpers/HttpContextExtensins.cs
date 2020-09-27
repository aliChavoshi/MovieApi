using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Helpers
{
    public static class HttpContextExtensins
    {
        public static async Task InsertPaginationParametersInResponse<T>(this HttpContext httpContext, IQueryable<T> queryable, int recordsPerPage)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }

            //Count List
            double count = await queryable.CountAsync();
            //totalAmoutPages
            double totalAmoutPages = Math.Ceiling(count / recordsPerPage);
            //Insert To Header 
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            httpContext.Response.Headers.Add("totalAmountPage", totalAmoutPages.ToString());

        }
    }
}