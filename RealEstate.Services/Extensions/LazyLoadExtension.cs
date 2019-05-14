using RealEstate.Services.BaseLog;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RealEstate.Services.Extensions
{
    public static class LazyLoadExtension
    {
        public static Lazy<T> LazyLoad<T>(Func<T> func)
        {
            return new Lazy<T>(func, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public static T LazyLoadLast<T>(this Lazy<List<T>> model) where T : BaseLogViewModel
        {
            var result = model.IsValueCreated ? model.Value.Last() : default;
            return result;
        }
    }
}