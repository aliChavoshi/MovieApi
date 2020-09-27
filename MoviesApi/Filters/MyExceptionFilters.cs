using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MoviesApi.Filters
{
    public class MyExceptionFilters : ExceptionFilterAttribute
    {
        private readonly ILogger<MyActionFilter> _logger;

        public MyExceptionFilters(ILogger<MyActionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            //فقط زمانی که خطا رخ دهد این بخش فراخوانی میشود 
            _logger.LogError(context.Exception, context.Exception.Message);

            base.OnException(context);
        }
    }
}