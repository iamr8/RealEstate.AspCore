using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using BaseEntity = RealEstate.Services.Database.Base.BaseEntity;

namespace RealEstate.Services.BaseLog
{
    public class BaseLogViewModel<TEntity> : BaseViewModel where TEntity : BaseEntity
    {
        [JsonIgnore]
        public readonly TEntity Entity;

        protected BaseLogViewModel(TEntity entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public BaseLogViewModel()
        {
        }

        [JsonIgnore]
        public LogViewModel Logs
        {
            get
            {
                var logs = Entity.Audits;
                var processedLog = new LogViewModel();
                if (logs.Count == 0)
                    return default;

                var users = new List<LogUserViewModel>();

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
                    var alreadyAddedUser = users.Find(x => x.Id == log.UserId);
                    if (alreadyAddedUser == null)
                    {
                        users.Add(new LogUserViewModel
                        {
                            Mobile = log.UserMobile,
                            FullName = log.UserFullName,
                            Id = log.UserId,
                        });
                    }

                    var mustBeAdded = new LogDetailViewModel
                    {
                        Type = log.Type,
                        DateTime = log.DateTime,
                        User = users.Find(x => x.Id == log.UserId)
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

        [JsonIgnore]
        public bool IsDeleted => Logs?.Last()?.Type == LogTypeEnum.Delete;
    }

    public static class BaseTrackExtension
    {
        public static LogDetailViewModel Last(this LogViewModel log)
        {
            var populate = new List<LogDetailViewModel>();

            if (log.Create != null)
                populate.Add(log.Create);

            if (log.Modifies?.Any() == true)
                populate.AddRange(log.Modifies);

            if (log.Deletes?.Any() == true)
                populate.AddRange(log.Deletes);

            populate = populate.OrderByDescending(x => x.DateTime).ToList();
            return populate.FirstOrDefault();
        }
    }

    public class LogViewModel
    {
        public LogDetailViewModel Create { get; set; }
        public List<LogDetailViewModel> Modifies { get; set; }
        public List<LogDetailViewModel> Deletes { get; set; }
    }

    public class LogDetailViewModel
    {
        public LogTypeEnum Type { get; set; }
        public DateTime DateTime { get; set; }
        public LogUserViewModel User { get; set; }
    }

    public class LogUserViewModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Mobile { get; set; }
    }
}