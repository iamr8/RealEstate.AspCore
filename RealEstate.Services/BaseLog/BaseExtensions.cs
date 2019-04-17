using RealEstate.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.BaseLog
{
    public static class BaseExtensions
    {
        public static TModel Into<TEntity, TModel>(this TEntity model, bool includeDeleted = false, Action<TModel> action = null) where TModel : class
        {
            if (model == null)
                return default;

            var ty = Activator.CreateInstance(typeof(TModel), model, includeDeleted, action) as TModel;
            return ty;
        }

        public static List<TModel> Into<TEntity, TModel>(this ICollection<TEntity> model, bool includeDeleted = false, Action<TModel> action = null) where TModel : class
        {
            if (model?.Any() != true)
                return default;

            var result = model
                .Select(entity => entity.Into(includeDeleted, action))
                .Where(x => x != null)
                .R8ToList();
            return result;
        }

        public static List<TModel> Into<TEntity, TModel>(this List<TEntity> model, bool includeDeleted = false, Action<TModel> action = null) where TModel : class
        {
            if (model?.Any() != true)
                return default;

            var result = model
                .Select(entity => entity.Into(includeDeleted, action))
                .Where(x => x != null)
                .R8ToList();
            return result;
        }
    }
}