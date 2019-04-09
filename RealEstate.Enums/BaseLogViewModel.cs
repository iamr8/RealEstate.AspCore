using Newtonsoft.Json;
using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Base
{
    public class BaseLogViewModel<TEntity> : BaseViewModel where TEntity : class
    {
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
        public LogViewModel Logs { get; set; }

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
        public string Id { get; set; }
        public LogUserViewModel User { get; set; }
    }

    public class LogUserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public Role Role { get; set; }
    }
}