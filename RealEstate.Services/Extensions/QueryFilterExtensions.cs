using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class QueryFilterExtensions
    {
        public static IOrderedQueryable<TSource> OrderDescendingByCreationDateTime<TSource>(this IQueryable<TSource> entities) where TSource : BaseEntity
        {
            var source = entities.OrderByDescending(x => x.Audits.FirstOrDefault(v => v.Type == LogTypeEnum.Create).DateTime);
            return source;
        }

        public static TModel Map<TEntity, TModel>(this TEntity model) where TEntity : BaseEntity where TModel : BaseLogViewModel
        {
            if (model == null)
                return default;

            var ty = Activator.CreateInstance(typeof(TModel), model) as TModel;
            return ty;
        }

        public static T Last<T>(this List<T> list) where T : BaseLogViewModel
        {
            var result = list?.OrderDescendingByCreationDateTime().FirstOrDefault();
            return result;
        }

        public static IOrderedEnumerable<TSource> OrderDescendingByCreationDateTime<TSource>(this ICollection<TSource> sources) where TSource : BaseEntity
        {
            var source = sources
                .OrderByDescending(x => x.Audits.FirstOrDefault(v => v.Type == LogTypeEnum.Create).DateTime);

            return source;
        }

        public static IOrderedEnumerable<TModel> OrderByCreationDateTime<TModel>(this List<TModel> sources) where TModel : BaseLogViewModel
        {
            var source = sources.OrderBy(x => x.Logs.Create.DateTime);
            return source;
        }

        public static IOrderedEnumerable<TModel> OrderDescendingByCreationDateTime<TModel>(this List<TModel> sources) where TModel : BaseLogViewModel
        {
            var source = sources.OrderByDescending(x => x.Logs.Create.DateTime);
            return source;
        }

        public static List<TModel> Map<TEntity, TModel>(this ICollection<TEntity> model) where TEntity : BaseEntity where TModel : BaseLogViewModel
        {
            if (model?.Any() != true)
                return default;

            var result = model
                .Select(entity => entity.Map<TEntity, TModel>())
                .Where(x => x?.Id != null)
                .R8ToList();
            return result;
        }

        public static List<TModel> Map<TEntity, TModel>(this List<TEntity> model) where TEntity : BaseEntity where TModel : BaseLogViewModel
        {
            if (model?.Any() != true)
                return default;

            var result = model
                .Select(entity => entity.Map<TEntity, TModel>())
                .Where(x => x != null)
                .R8ToList();
            return result;
        }

        public static IList<TEntity> WhereNotDeleted<TEntity>(this ICollection<TEntity> entities) where TEntity : BaseEntity
        {
            if (entities?.Any() != true)
                return default;

            var result = entities.Where(entity => string.IsNullOrEmpty(entity.Audit)
                                                  || entity.Audits.OrderByDescending(x => x.DateTime).FirstOrDefault().Type != LogTypeEnum.Delete);
            return result.ToList();
        }

        public static EntityTypeBuilder<TEntity> WhereNotDeleted<TEntity>(this EntityTypeBuilder<TEntity> entities) where TEntity : BaseEntity
        {
            var result = entities.HasQueryFilter(entity => string.IsNullOrEmpty(entity.Audit)
                                                  || entity.Audits.OrderByDescending(x => x.DateTime).FirstOrDefault().Type != LogTypeEnum.Delete);
            return result;
        }

        public static IQueryable<TEntity> WhereNotDeleted<TEntity>(this IQueryable<TEntity> entities) where TEntity : BaseEntity
        {
            var result = entities.Where(entity => string.IsNullOrEmpty(entity.Audit)
                                                  || entity.Audits.OrderByDescending(x => x.DateTime).FirstOrDefault().Type != LogTypeEnum.Delete);
            return result;
        }
    }
}