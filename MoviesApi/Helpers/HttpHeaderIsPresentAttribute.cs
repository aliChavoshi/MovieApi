using System;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace MoviesApi.Helpers
{
    //IActionConstraint :اقدام محدودیت
    public class HttpHeaderIsPresentAttribute : Attribute, IActionConstraint
    {
        public int Order => 0;
        private readonly string _header;
        private readonly string _value;

        //مقداری که بالای کنترلر دادم در اینجا نمایش داده میشود
        public HttpHeaderIsPresentAttribute(string header, string value)
        {
            _header = header;
            _value = value;
        }

        public bool Accept(ActionConstraintContext context)
        {
            //هدر کل درخواست ها
            var headers = context.RouteContext.HttpContext.Request.Headers;
            //اگر هدر من توی لیستی از هدر ها نبود
            if (!headers.ContainsKey(_header))
            {
                return false;
            }
            //اگر باهم یکی بودند مقدار True 
            //اگر باهم برابر نبودند مقدار False
            return string.Equals(headers[_header], _value, StringComparison.OrdinalIgnoreCase);
        }


    }
}