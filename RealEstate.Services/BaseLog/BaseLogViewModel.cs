using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.BaseLog
{
    public class BaseLogViewModel
    {
        public string Id
        {
            get
            {
                var type = GetType();
                var properties = type.GetProperties().ToList();
                if (properties?.Any() != true)
                    return default;

                var entityProp = properties.Find(x => x.Name.Equals("Entity", StringComparison.CurrentCulture));
                if (entityProp?.PropertyType.IsSubclassOf(typeof(BaseEntity)) != true)
                    return default;

                var entity = entityProp.GetValue(this);
                if (entity == null)
                    return default;

                var idProp = entityProp.PropertyType.GetPublicProperties().Find(x => x.Name.Equals("Id"));
                if (idProp == null)
                    return default;

                var id = idProp.GetValue(entity) as string;
                return id;
            }
        }

        [JsonIgnore]
        public LogViewModel Logs
        {
            get
            {
                var type = GetType();
                var properties = type.GetProperties().ToList();
                if (properties?.Any() != true)
                    return default;

                var entityProp = properties.Find(x => x.Name.Equals("Entity", StringComparison.CurrentCulture));
                if (entityProp?.PropertyType.IsSubclassOf(typeof(BaseEntity)) != true)
                    return default;

                var entity = entityProp.GetValue(this);
                if (entity == null)
                    return default;

                var auditsProp = entityProp.PropertyType.GetPublicProperties().Find(x => x.Name.Equals("Audits"));
                if (auditsProp == null)
                    return default;

                var audits = auditsProp.GetValue(entity) as List<LogJsonEntity>;
                if (audits?.Any() != true)
                    return default;

                var processedLog = audits.Render();
                return processedLog;
            }
        }

        [JsonIgnore]
        public bool IsDeleted => Logs?.Last?.Type == LogTypeEnum.Delete;
    }

    public class LogViewModel
    {
        public LogDetailViewModel Create { get; set; }
        public List<LogDetailViewModel> Modifies { get; set; }
        public List<LogDetailViewModel> Deletes { get; set; }

        public LogDetailViewModel Last
        {
            get
            {
                var populate = new List<LogDetailViewModel>();

                if (Create != null)
                    populate.Add(Create);

                if (Modifies?.Any() == true)
                    populate.AddRange(Modifies);

                if (Deletes?.Any() == true)
                    populate.AddRange(Deletes);

                populate = populate.OrderByDescending(x => x.DateTime).ToList();
                return populate.FirstOrDefault();
            }
        }
    }

    public class LogDetailViewModel
    {
        public LogTypeEnum Type { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }

        public string UserFullName { get; set; }

        public string UserMobile { get; set; }
        public Dictionary<string, string> Modifies { get; set; }
    }
}