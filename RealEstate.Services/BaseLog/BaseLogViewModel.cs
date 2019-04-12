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
        private readonly TEntity _entity;

        protected BaseLogViewModel(TEntity entity)
        {
            _entity = entity;
            if (_entity == null)
                return;
        }

        public BaseLogViewModel()
        {
        }

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
        public LogUserViewModel User { get; set; }
    }

    public class LogUserViewModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Mobile { get; set; }
    }
}