using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using MoviesApi.DTOs;

namespace MoviesApi.Helpers
{
    public class LinksGenerator
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        public LinksGenerator(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper GetUrlHelper()
        {
            //مقدار url  را از context  برای من بدست میاورد
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        }

        //دقیقا ورودی های متد GenreAttribute  را میگیرد تا در اون کلاس متد های زیر در دسترس باشد
        public async Task Genereate<T>(ResultExecutingContext context, ResultExecutionDelegate next) where T : class, IGenerateHateoasLinks, new()
        {
            //Get Url
            var urlHelper = GetUrlHelper();
            //get Context Result
            var result = context.Result as ObjectResult;
            //یک مدل تنها یعنی لیست نیست
            var model = result.Value as T;
            //در زمان کالکشن چون لیست میباشد پس نال را بزگشت خواهد داد پس باید بریک یک نمونه از جنس لیست بسازیم
            if (model == null)
            {
                //این بخش برای کالکشن ها است
                //یک لیست میسازم
                var modelList = result.Value as List<T> ??
                                throw new ArgumentNullException($"Was Expecting an instance of {typeof(T)}");
                //لینک های لیست را واسم بساز
                //دقیقا مثل esle  میباشد فقط در یک حلقه اجرا شده است
                modelList.ForEach(dto => dto.GenerateLinks(urlHelper));
                //در بالا از مقدار جدید هم ارث بریک کردمی :  new ()
                //نمونه جدید میسازیم
                var indivisual = new T();
                //میخواهیم برای حالت کلی هم بسازیم
                //در این بخش جنس مقدار بازشگتی به دو بخش تقسیم میشود یکی مقدار values  و دیگری مقدار links 
                //بات وجه به متد کالشکن که مقدار های ورودی را به دوبخش تقسیم میکند
                result.Value = indivisual.GenerateLinksCollection(modelList, urlHelper);
                //Run
                await next();
            }
            else
            {
                //این بخش برای تکی ها و غیره میباشد
                //اگر مدلی وجود داشت باید بریم لینک ها را بسازیم
                //GenerateLinks :  این متد از اینترفسی میاد که در بالا این متد ازش ارث بری کرده است
                model.GenerateLinks(urlHelper);
                await next();
            }
        }

    }
}