using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Base
{
    public static class BaseIncludeExtensions
    {
        public static TModel R8Include<TModel>(this TModel source, Action<TModel> property)
        {
            if (source == null)
                return default;

            property.Invoke(source);
            return source;
        }

        public static List<TModel> R8ToList<TModel>(this IEnumerable<TModel> collection)
        {
            return collection.Where(x => x != null).ToList();
        }

        public static TModel ShowBasedOn<TModel, TCondition>(this TModel model, Func<TModel, TCondition> condition) where TModel : class where TCondition : class
        {
            if (model == null)
                return default;

            var cond = condition.Invoke(model);
            return cond == null ? default : model;

        }
    }
}