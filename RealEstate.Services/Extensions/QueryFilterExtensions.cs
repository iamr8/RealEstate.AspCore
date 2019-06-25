using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore.Internal;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class QueryFilterExtensions
    {
        public static IOrderedQueryable<TSource> OrderDescendingByCreationDateTime<TSource>(this IQueryable<TSource> entities) where TSource : BaseEntity
        {
            var source = from que in entities
                         orderby CustomDbFunctions.JsonValue(que.Audit, "$[0].d") descending
                         select que;
            return source;
        }

        public static string ToSql<TSource>(this IQueryable<TSource> query)
        {
            return query.ToSql(query.Expression, new EFCacheKeyHashProvider())?.Sql;
        }

        public static IOrderedQueryable<TSource> OrderByCreationDateTime<TSource>(this IQueryable<TSource> entities) where TSource : BaseEntity
        {
            var source = from que in entities
                         orderby CustomDbFunctions.JsonValue(que.Audit, "$[0].d")
                         select que;
            return source;
        }

        public static IOrderedEnumerable<TSource> OrderDescendingByCreationDateTime<TSource>(this ICollection<TSource> sources) where TSource : BaseEntity
        {
            var source = sources
                .OrderByDescending(x => x.Audits.FirstOrDefault(v => v.Type == LogTypeEnum.Create).DateTime);

            return source;
        }

        public static IOrderedEnumerable<TModel> OrderDescendingByCreationDateTime<TModel>(this List<TModel> sources) where TModel : BaseLogViewModel
        {
            var source = sources.OrderByDescending(x => x.Logs.Create.DateTime);
            return source;
        }

        public static List<TModel> Map<TEntity, TModel>(this List<TEntity> model) where TEntity : BaseEntity where TModel : BaseLogViewModel
        {
            if (model?.Any() != true)
                return default;

            var result = model
                .Select(entity => entity.Map<TModel>())
                .Where(x => x != null)
                .ToHasNotNullList();
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
    }
}