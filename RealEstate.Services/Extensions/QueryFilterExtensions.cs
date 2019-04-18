using RealEstate.Base;
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
        public static TModel Into<TEntity, TModel>(this TEntity model, bool includeDeleted = false, Action<TModel> action = null) where TModel : class
        {
            if (model == null)
                return default;

            var ty = Activator.CreateInstance(typeof(TModel), model, includeDeleted, action) as TModel;
            return ty;
        }

        public static IOrderedEnumerable<TSource> OrderDescendingByCreationDateTime<TSource>(this ICollection<TSource> sources) where TSource : BaseEntity
        {
            var source = sources.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime);
            return source;
        }

        public static IOrderedQueryable<TSource> OrderDescendingByCreationDateTime<TSource>(this IQueryable<TSource> sources) where TSource : BaseEntity
        {
            var source = sources.OrderByDescending(x => x.Audits.Find(v => v.Type == LogTypeEnum.Create).DateTime);
            return source;
        }

        public static IOrderedEnumerable<TModel> OrderDescendingByCreationDateTime<TModel>(this List<TModel> sources) where TModel : BaseLogViewModel
        {
            var source = sources.OrderByDescending(x => x.Logs.Create.DateTime);
            return source;
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

        public static IQueryable<TEntity> Filtered<TEntity>(this IQueryable<TEntity> entities) where TEntity : BaseEntity
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