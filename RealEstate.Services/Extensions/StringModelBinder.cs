using Microsoft.AspNetCore.Mvc.ModelBinding;
using RealEstate.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services.Extensions
{
    public static class StringExtensions
    {
        public static string Replace(this string str, IEnumerable<string> oldValues, string newValue)
        {
            return oldValues?.Any() != true
                ? str
                : oldValues.Aggregate(str, (current, oldValue) => current.Replace(oldValue, newValue));
        }
    }

    public class StringModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(string))
                return new StringModelBinder();

            return null;
        }
    }

    public class StringModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            if (bindingContext.ModelType != typeof(string))
                return Task.FromResult(ModelBindingResult.Failed());

            var key = bindingContext.ModelName;
            if (string.IsNullOrEmpty(key))
                return Task.CompletedTask;

            var getValue = bindingContext.ValueProvider.GetValue(key);

            var value = getValue.FirstValue;
            if (string.IsNullOrEmpty(value))
                return Task.CompletedTask;

            value = value.FixNumbers().Trim();
            bindingContext.Result = ModelBindingResult.Success(value);
            return Task.CompletedTask;
        }
    }
}