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
        /// <summary>
        /// Returns specific navigation property from entity to model
        /// </summary>
        /// <typeparam name="TSource">EntityFramework Core entity based on " BaseEntity "</typeparam>
        /// <typeparam name="TModel">Output ViewModel based on " BaseLogViewModel "</typeparam>
        /// <param name="entity">Entity ( subclass of BaseEntity )</param>
        /// <param name="action">Action to do on viewmodel</param>
        public TModel IncludeAs<TSource, TModel>(TSource entity, Action<TModel> action = null) where TModel : BaseLogViewModel where TSource : BaseEntity
        {
            if (entity == null)
                return default;

            var thisProperty = GetType().GetPublicProperties()
                .FirstOrDefault(x => x.PropertyType == typeof(TModel));
            if (thisProperty == null)
                return default;

            if (thisProperty.GetValue(this) is TModel currentValue)
                return default;

            var value = Activator.CreateInstance(typeof(TModel), entity, action) as TModel;
            thisProperty.SetValue(this, value);
            return value;
        }

        /// <summary>
        /// Returns specific navigation property from entity to model
        /// </summary>
        /// <typeparam name="TSource">EntityFramework Core entity based on " BaseEntity "</typeparam>
        /// <typeparam name="TModel">Output ViewModel based on " BaseLogViewModel "</typeparam>
        /// <param name="entity">Entity ( subclass of BaseEntity )</param>
        /// <param name="action">Action to do on viewmodel</param>
        public List<TModel> IncludeAs<TSource, TModel>(ICollection<TSource> entity, Action<TModel> action = null) where TModel : BaseLogViewModel where TSource : BaseEntity
        {
            if (entity?.Any() != true)
                return default;

            var thisProperty = GetType().GetPublicProperties()
                .FirstOrDefault(x => x.PropertyType == typeof(List<TModel>));
            if (thisProperty == null)
                return default;

            var values = entity.Select(source => Activator.CreateInstance(typeof(TModel), source, action) as TModel).ToList();
            thisProperty.SetValue(this, values);
            return values;
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