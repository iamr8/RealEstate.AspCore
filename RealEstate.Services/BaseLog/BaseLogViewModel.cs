using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RealEstate.Services.BaseLog
{
    public class BaseLogViewModel
    {
        private PropertyInfo EntityProperty
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

                return entityProp;
            }
        }

        public string Id
        {
            get
            {
                BaseEntity baser;
                var entity = EntityProperty?.GetValue(this);
                if (entity == null)
                    return default;

                var idProp = EntityProperty?.PropertyType.GetPublicProperties().Find(x => x.Name.Equals(nameof(baser.Id)));
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
                BaseEntity baser;
                var entity = EntityProperty?.GetValue(this);
                if (entity == null)
                    return default;

                var auditsProp = EntityProperty?.PropertyType.GetPublicProperties().Find(x => x.Name.Equals(nameof(baser.Audits)));
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
}