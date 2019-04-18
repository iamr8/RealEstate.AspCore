using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.BaseLog
{
    public class BaseLogViewModel : BaseViewModel
    {
        [JsonIgnore]
        public LogViewModel Logs { get; set; }

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
        public string UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserMobile { get; set; }
    }
}