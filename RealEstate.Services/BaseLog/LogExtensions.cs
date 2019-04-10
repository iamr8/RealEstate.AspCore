using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Domain.Base;
using RealEstate.Domain.Tables;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.BaseLog
{
    public static class LogExtensions
    {
        public static TModel AddLogs<TModel, TEntity>(this TModel model, TEntity entity, IQueryable<User> dbUsers)
            where TModel : BaseLogViewModel<TEntity> where TEntity : BaseEntity
        {
            return model.Include(v => v.Logs = entity.Logs.Map(dbUsers, null));
        }

        public static List<TModel> Filtered<TModel>(this List<TModel> models) where TModel : class
        {
            if (models?.Any() != true)
                return default;

            var result = new List<TModel>();
            foreach (var model in models)
            {
                var ff = model.Filtered();
                if (ff == null)
                    continue;

                result.Add(ff);
            }

            return result;
        }

        public static TModel Filtered<TModel>(this TModel model) where TModel : class
        {
            if (model == null)
                return default;

            var type = model.GetType();

            var deletedProp = type.GetProperty("IsDeleted");
            if (deletedProp == null)
                return model;

            var valueProp = deletedProp.GetValue(model);
            if (valueProp != null && valueProp is bool deleted)
                return deleted ? model : default;

            return default;
        }

        private static LogViewModel Map(this ICollection<Log> logs, IQueryable<User> dbUsers, List<LogUserViewModel> users)
        {
            var processedLog = new LogViewModel();
            if (logs.Count == 0)
                return default;

            if (users?.Any() != true)
                users = new List<LogUserViewModel>();

            var create = logs.FirstOrDefault(x => x.Type == LogTypeEnum.Create);
            var modifies = logs.OrderByDescending(x => x.DateTime).Where(x => x.Type == LogTypeEnum.Modify || x.Type == LogTypeEnum.Undelete).ToList();
            var deletes = logs.OrderByDescending(x => x.DateTime).Where(x => x.Type == LogTypeEnum.Delete).ToList();

            var logsList = new List<Log>();

            if (create != null)
                logsList.Add(create);

            if (modifies?.Any() == true)
                logsList.AddRange(modifies);

            if (deletes?.Any() == true)
                logsList.AddRange(deletes);

            logsList = logsList.OrderByDescending(x => x.DateTime).ToList();
            var packedTracks = from log in logsList
                               join creator in dbUsers on log.CreatorId equals creator.Id
                               select new
                               {
                                   Log = log,
                                   Creator = creator
                               };

            foreach (var packedTrack in packedTracks)
            {
                var thisUser = packedTrack.Creator;
                var thisLog = packedTrack.Log;

                var alreadyAddedUser = users.Find(x => x.Id == packedTrack.Creator.Id);
                if (alreadyAddedUser == null)
                {
                    users.Add(new LogUserViewModel
                    {
                        FirstName = thisUser.FirstName,
                        LastName = thisUser.LastName,
                        Mobile = thisUser.Mobile,
                        Role = thisUser.Role,
                        Username = thisUser.Username,
                        Id = thisUser.Id
                    });
                }

                var mustBeAdded = new LogDetailViewModel
                {
                    Type = thisLog.Type,
                    DateTime = thisLog.DateTime,
                    Id = thisLog.Id,
                    User = users.Find(x => x.Id == thisUser.Id)
                };

                switch (thisLog.Type)
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