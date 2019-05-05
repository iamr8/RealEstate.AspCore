using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RealEstate.Services.Extensions
{
    public static class QueryFilterExtensions
    {
        public static IOrderedQueryable<TSource> OrderDescendingByCreationDateTime<TSource>(this IQueryable<TSource> entities) where TSource : BaseEntity
        {
            var source = entities.OrderByDescending(x => x.Audits.FirstOrDefault(v => v.Type == LogTypeEnum.Create).DateTime);
            return source;
        }

        public static TModel Into<TEntity, TModel>(this TEntity model, bool includeDeleted = false, Action<TModel> action = null) where TEntity : BaseEntity where TModel : BaseLogViewModel
        {
            if (model == null)
                return default;

            var ignoreDeletedItems = !includeDeleted;
            var currentDeleteState = model.IsDeleted;
            if (ignoreDeletedItems && currentDeleteState)
                return default;

            var ty = Activator.CreateInstance(typeof(TModel), model, includeDeleted, action) as TModel;
            return ty;
        }

        //public static IQueryable<TSource> SearchBy<TSource>(this IQueryable<TSource> source, string condition, Expression<Func<TSource, string>> expression) where TSource : BaseEntity
        //{
        //    if (condition == null)
        //        return source;

        //    source = source.Where(x => EF.Functions.Like(expression.Compile().Invoke(x), condition.LikeExpression()));
        //    return source;
        //}
        public static IQueryable<TSource> SearchBy<TSource, TType>(this IQueryable<TSource> source, TType condition, Expression<Func<TSource, TType>> expression) where TSource : BaseEntity
        {
            if (condition == null)
                return source;

            if (condition is string condString)
            {
                if (string.IsNullOrEmpty(condString))
                    return source;

                source = source.Where(x => EF.Functions.Like(expression.Compile().Invoke(x).ToString(), condString.Like()));
            }
            else
            {
                source = source.Where(x => expression.Compile().Invoke(x).Equals(condition));
            }
            return source;
        }

        public static IQueryable<TSource> SearchBy<TSource, TType>(this IQueryable<TSource> source, TType condition, Expression<Func<TSource, TType, bool>> expression) where TSource : BaseEntity
        {
            if (condition == null)
                return source;

            if (condition is string condString)
            {
                if (string.IsNullOrEmpty(condString))
                    return source;

                source = source.Where(x => EF.Functions.Like(expression.Compile().Invoke(x, condition).ToString(), condString.Like()));
            }
            else
            {
                source = source.Where(x => expression.Compile().Invoke(x, condition).Equals(condition));
            }
            return source;
        }

        public static IOrderedEnumerable<TSource> OrderDescendingByCreationDateTime<TSource>(this ICollection<TSource> sources) where TSource : BaseEntity
        {
            var source = sources
                .OrderByDescending(x => x.Audits.FirstOrDefault(v => v.Type == LogTypeEnum.Create).DateTime);

            return source;
        }

        // query.Where(x => x.Deals.OrderDescendingByCreationDateTime().FirstOrDefault().Status != DealStatusEnum.Finished);

        //var sourceConstantExpression = (ConstantExpression)entities.Expression;
        //var sourceQueryProvider = entities.Provider; // EntityQueryProvider.

        //// Expression<Func<Product, bool>> predicateExpression = product => product.Name.Length > 10;
        //var productParameterExpression = Expression.Parameter(typeof(TSource), "entity");
        //var predicateExpression = Expression.Lambda<Func<TSource, bool>>(
        //    body: Expression.GreaterThan(
        //        left: Expression.Property(
        //            expression: Expression.Property(
        //                expression: productParameterExpression, propertyName: "Audit"),
        //            propertyName: nameof(string.Length)),
        //        right: Expression.Constant(10)),
        //    parameters: productParameterExpression);

        //// IQueryable<Product> whereQueryable = sourceQueryable.Where(predicateExpression);
        //Func<IQueryable<TSource>, Expression<Func<TSource, bool>>, IQueryable<TSource>> whereMethod =
        //    Queryable.Where;
        //var whereCallExpression = Expression.Call(
        //    method: whereMethod.Method,
        //    arg0: sourceConstantExpression,
        //    arg1: Expression.Quote(predicateExpression));
        //var whereQueryable = sourceQueryProvider
        //    .CreateQuery<TSource>(whereCallExpression); // EntityQueryable<Product>.
        //var whereQueryProvider = whereQueryable.Provider; // EntityQueryProvider.

        //// Expression<Func<Product, string>> selectorExpression = product => product.Name;
        //var selectorExpression = Expression.Lambda<Func<TSource, string>>(
        //    body: Expression.Property(productParameterExpression, "Audit"),
        //    parameters: productParameterExpression);

        //// IQueryable<string> selectQueryable = whereQueryable.Select(selectorExpression);
        //Func<IQueryable<TSource>, Expression<Func<TSource, string>>, IQueryable<string>> selectMethod =
        //    Queryable.Select;
        //var selectCallExpression = Expression.Call(
        //    method: selectMethod.Method,
        //    arg0: whereCallExpression,
        //    arg1: Expression.Quote(selectorExpression));
        //var selectQueryable = whereQueryProvider
        //    .CreateQuery<string>(selectCallExpression); // EntityQueryable<Product>/DbQuery<Product>.

        //using (IEnumerator<string> iterator = selectQueryable.GetEnumerator()) // Execute query.
        //{
        //    while (iterator.MoveNext())
        //    {
        //        //                    iterator.Current.WriteLine();
        //    }
        //}
        public static IOrderedEnumerable<TModel> OrderDescendingByCreationDateTime<TModel>(this List<TModel> sources) where TModel : BaseLogViewModel
        {
            var source = sources.OrderByDescending(x => x.Logs.Create.DateTime);
            return source;
        }

        public static List<TModel> Into<TEntity, TModel>(this ICollection<TEntity> model, bool includeDeleted = false, Action<TModel> action = null) where TEntity : BaseEntity where TModel : BaseLogViewModel
        {
            if (model?.Any() != true)
                return default;

            var result = model
                .Select(entity => entity.Into(includeDeleted, action))
                .Where(x => x?.Id != null)
                .R8ToList();
            return result;
        }

        public static List<TModel> Into<TEntity, TModel>(this List<TEntity> model, bool includeDeleted = false, Action<TModel> action = null) where TEntity : BaseEntity where TModel : BaseLogViewModel
        {
            if (model?.Any() != true)
                return default;

            var result = model
                .Select(entity => entity.Into(includeDeleted, action))
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

        public static LogJsonEntity LastLog<TEntity>(this TEntity entity) where TEntity : BaseEntity
        {
            var result = entity.Audits?.OrderByDescending(x => x.DateTime).FirstOrDefault();
            return result;
        }

        public static LogViewModel GetLogs<TModel>(this TModel entity) where TModel : BaseEntity
        {
            if (entity?.Audits == null)
                return default;

            var logs = entity.Audits?.ToList();
            if (logs.Count == 0)
                return default;

            var processedLog = new LogViewModel();
            var create = logs.Find(x => x.Type == LogTypeEnum.Create);
            var modifies = logs.OrderByDescending(x => x.DateTime).Where(x => x.Type == LogTypeEnum.Modify || x.Type == LogTypeEnum.Undelete).ToList();
            var deletes = logs.OrderByDescending(x => x.DateTime).Where(x => x.Type == LogTypeEnum.Delete).ToList();

            var logsList = new List<LogJsonEntity>();

            if (create != null)
                logsList.Add(create);

            if (modifies.Count > 0)
                logsList.AddRange(modifies);

            if (deletes.Count > 0)
                logsList.AddRange(deletes);

            logsList = logsList.OrderByDescending(x => x.DateTime).ToList();
            foreach (var log in logsList)
            {
                var mustBeAdded = new LogDetailViewModel
                {
                    Type = log.Type,
                    DateTime = log.DateTime,
                    UserId = log.UserId,
                    UserMobile = log.UserMobile,
                    UserFullName = log.UserFullName
                };

                switch (log.Type)
                {
                    case LogTypeEnum.Create:
                        processedLog.Create = mustBeAdded;
                        break;

                    case LogTypeEnum.Delete:
                        if (processedLog.Deletes?.Any() != true)
                            processedLog.Deletes = new List<LogDetailViewModel>();

                        processedLog.Deletes.Add(mustBeAdded);
                        break;

                    case LogTypeEnum.Modify:
                    case LogTypeEnum.Undelete:
                        if (processedLog.Modifies?.Any() != true)
                            processedLog.Modifies = new List<LogDetailViewModel>();

                        processedLog.Modifies.Add(mustBeAdded);
                        break;
                }
            }

            return processedLog;
        }
    }
}