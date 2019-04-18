using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Base
{
    public static class BaseIncludeExtensions
    {
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