using RealEstate.Base;
using System;
using System.Linq.Expressions;
using System.Text;

namespace RealEstate.Services.Extensions
{
    public static class CacheKeyExtensions
    {
        /// <summary>
        /// Append key to cache concurrency string builder to use in .Cacheable()
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="stringBuilder"></param>
        /// <param name="searchModel"></param>
        /// <param name="expression"></param>
        public static void AppendKey<TModel>(this StringBuilder stringBuilder, TModel searchModel, Expression<Func<TModel, object>> expression) where TModel : BaseSearchModel
        {
            //var hasValue = !string.IsNullOrEmpty(stringBuilder.ToString());
            //var prefix = hasValue ? "&&" : "";

            //var key = expression.GetMemberName();
            //var value = expression.Compile().Invoke(searchModel);

            //if (value != null)
            //    stringBuilder.Append($"{prefix}{key}=={value}");
        }

        public static void AppendKey(this StringBuilder stringBuilder, string key, object value)
        {
            if (string.IsNullOrEmpty(key) && value != null)
                return;

            var hasValue = !string.IsNullOrEmpty(stringBuilder.ToString());
            var prefix = hasValue ? "&&" : "";

            stringBuilder.Append($"{prefix}{key}=={value}");
        }
    }
}