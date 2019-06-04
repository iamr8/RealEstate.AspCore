using RealEstate.Services.Database.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RealEstate.Services.BaseLog
{
    public static class Extensions
    {
        public static List<TModel> ToHasNotNullList<TModel>(this IEnumerable<TModel> collection) where TModel : BaseLogViewModel
        {
            return collection.Where(x => x != null).ToList();
        }

        public static PropertyInfo GetEntity(this Type classType)
        {
            var type = classType;
            var properties = type.GetProperties().ToList();
            if (properties?.Any() != true)
                return default;

            var entityProp = properties.Find(x => x.Name.Equals("Entity", StringComparison.CurrentCulture));
            if (entityProp?.PropertyType.IsSubclassOf(typeof(BaseEntity)) != true)
                return default;

            return entityProp;
        }

        public static TModel ShowBasedOn<TModel, TCondition>(this TModel model, Func<TModel, TCondition> condition) where TModel : BaseLogViewModel where TCondition : BaseLogViewModel
        {
            if (model == null)
                return default;

            var cond = condition.Invoke(model);
            return cond?.IsDeleted == true ? default : model;
        }

        public static List<TModel> ShowBasedOn<TModel, TCondition>(this List<TModel> models, Func<TModel, TCondition> condition) where TModel : BaseLogViewModel where TCondition : BaseLogViewModel
        {
            if (models?.Any() != true)
                return default;

            var result = new List<TModel>();
            foreach (var model in models)
            {
                var item = model.ShowBasedOn(condition);
                if (item == null)
                    continue;

                result.Add(item);
            }

            return result;
        }
    }
}