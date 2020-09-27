using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Internal;

namespace MoviesApi.Helpers
{
    public class HateosAttribute : ResultFilterAttribute
    {
        protected bool ShouldIncludeHateas(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;
            if (!IsSuccessFullResponse(result))
            {
                return false;
            }

            //درخواست را از کاربر دریافت کن
            var header = context.HttpContext.Request.Headers["IncludeHATEOAS"];
            //اصلا ببین درخواستی وجود دارد
            if (header.Count == 0)
            {
                return false;
            }
            //مقدار آن درخواست را بردار
            var value = header[0];
            //اگر درخواست مخالف با yes  بود بازهم بهش دسترسی نده
            //InvariantCultureIgnoreCase : حروف کوچک و بزرگ را در نظر نگیر
            if (!value.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            //غیر این صورت دسترسی خواهد داشت
            return true;
        }


        //باید چک کنیم که کانتکس من مقدار دارد یا خیر
        private static bool IsSuccessFullResponse(ObjectResult result)
        {
            //اگر مقداری نداشت منفی بازگشت بده
            if (result == null || result.Value == null)
            {
                return false;
            }
            //اگر مقدار داشت ولی موفقیت آمیز نبوده تباز هم مقدار منفی را بازگشت بده به من
            if (result.StatusCode.HasValue && !result.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }
            //در غیر این صورت تایید کن
            return true;
        }
    }
}