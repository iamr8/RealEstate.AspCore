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
        public void Include<TSource, TModel>(TSource entity, Action<TModel> act = null) where TModel : BaseLogViewModel where TSource : BaseEntity
        {
            if (entity == null)
                return;

            var thisProperty = GetType().GetPublicProperties()
                .FirstOrDefault(x => x.PropertyType == typeof(TModel));
            if (thisProperty == null)
                return;

            var value = Activator.CreateInstance(typeof(TModel), entity, act) as TModel;
            thisProperty.SetValue(this, value);
        }

        public void Include<TSource, TModel>(ICollection<TSource> entity, Action<TModel> act = null) where TModel : BaseLogViewModel where TSource : BaseEntity
        {
            if (entity?.Any() != true)
                return;

            var thisProperty = GetType().GetPublicProperties()
                .FirstOrDefault(x => x.PropertyType == typeof(List<TModel>));
            if (thisProperty == null)
                return;

            var result = entity.Select(source => Activator.CreateInstance(typeof(TModel), source, act) as TModel).ToList();
            thisProperty.SetValue(this, result);
        }

        public string Id
        {
            get
            {
                BaseEntity baser;
                var entity = GetType().GetEntity()?.GetValue(this);
                if (entity == null)
                    return default;

                var idProp = GetType().GetEntity()?.PropertyType.GetPublicProperties().Find(x => x.Name.Equals(nameof(baser.Id)));
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
                var entity = GetType().GetEntity()?.GetValue(this);
                if (entity == null)
                    return default;

                var auditsProp = GetType().GetEntity()?.PropertyType.GetPublicProperties().Find(x => x.Name.Equals(nameof(baser.Audits)));
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

    public class BaseLogViewModel<T> where T : BaseEntity
    {
        [JsonIgnore]
        public T Entity { get; set; }

        public string Id
        {
            get
            {
                var idProp = Entity.GetType().GetPublicProperties().Find(x => x.Name.Equals(nameof(Entity.Id), StringComparison.CurrentCulture));
                if (idProp == null)
                    return default;

                var id = idProp.GetValue(Entity) as string;
                return id;
            }
        }

        [JsonIgnore]
        public LogViewModel Logs
        {
            get
            {
                var auditsProp = Entity.GetType().GetPublicProperties().Find(x => x.Name.Equals(nameof(Entity.Audits), StringComparison.CurrentCulture));
                if (auditsProp == null)
                    return default;

                var audits = auditsProp.GetValue(Entity) as List<LogJsonEntity>;
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