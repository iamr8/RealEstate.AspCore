using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class LogExtensions
    {
        public static LogDetailViewModel MapLog(this LogJsonEntity log)
        {
            if (log == null)
                return default;

            var mustBeAdded = new LogDetailViewModel
            {
                Type = log.Type,
                DateTime = log.DateTime,
                UserId = log.UserId,
                UserMobile = log.UserMobile,
                UserFullName = log.UserFullName,
                Modifies = log.Type == LogTypeEnum.Modify
                    ? log.Modifies?.Select(x => new
                    {
                        x.Key,
                        x.Value
                    })?.ToDictionary(pair => pair.Key, pair => pair.Value)
                    : default
            };
            return mustBeAdded;
        }

        public static LogViewModel Render(this List<LogJsonEntity> audits)
        {
            var processedLog = new LogViewModel();
            var create = audits.Find(x => x.Type == LogTypeEnum.Create);
            var modifies = audits.OrderByDescending(x => x.DateTime).Where(x => x.Type == LogTypeEnum.Modify || x.Type == LogTypeEnum.Undelete).ToList();
            var deletes = audits.OrderByDescending(x => x.DateTime).Where(x => x.Type == LogTypeEnum.Delete).ToList();

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
                var mustBeAdded = log.MapLog();
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