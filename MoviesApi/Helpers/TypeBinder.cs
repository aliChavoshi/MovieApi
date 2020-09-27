using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace MoviesApi.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //ابتدا نامش را بدست میاوریم
            var propertyName = bindingContext.ModelName;
            //سپس ارزش آن را بدست میاوریم
            //بررسی میکنه ببینه اصلا مقدار داخل این وارد شده است یا خیر
            var valueProviderResult = bindingContext.ValueProvider.GetValue(propertyName);
            //اگرارزشی وجود نداشت کار انجام شده نیازی به ادامه نیست
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;

            }
            //مقدار آن را به آبجکت تبدیل کنید
            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<T>(valueProviderResult.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(propertyName, "value in invalid");
            }
            return Task.CompletedTask;
        }

    }
}