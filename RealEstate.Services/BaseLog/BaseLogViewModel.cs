using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace RealEstate.Services.BaseLog
{
    public class BaseLogViewModel
    {
        public TModel IncludeAs<TEntity, TSource, TModel>(IUnitOfWork unitOfWork, Expression<Func<TEntity, TSource>> property, Action<TModel> action = null) where TModel : BaseLogViewModel where TSource : BaseEntity where TEntity : BaseEntity
        {
            var entityProperty = GetType().GetPublicProperties().FirstOrDefault(x => x.Name.Equals("Entity", StringComparison.CurrentCulture));
            if (entityProperty == null)
                return default;

            if (!(entityProperty.GetValue(this) is TEntity entity))
                return default;

            var entry = unitOfWork.GetEntityEntry(entity);
            if (entry == null)
                return default;

            var reference2 = entry.Reference(property);
            var currValue = reference2.CurrentValue;
            if (currValue == null)
                return default;

            var thisProperty = GetType().GetPublicProperties()
                .FirstOrDefault(x => x.PropertyType == typeof(TModel));
            if (thisProperty == null)
                return default;

            if (thisProperty.GetValue(this) is TModel currentValue)
                return default;

            var value = Activator.CreateInstance(typeof(TModel), currValue, action) as TModel;
            thisProperty.SetValue(this, value);
            return value;
        }

        public List<TModel> IncludeAs<TEntity, TSource, TModel>(IUnitOfWork unitOfWork, Expression<Func<TEntity, IEnumerable<TSource>>> collection, Action<TModel> action = null) where TModel : BaseLogViewModel where TSource : BaseEntity where TEntity : BaseEntity
        {
            var entityProperty = GetType().GetPublicProperties().FirstOrDefault(x => x.Name.Equals("Entity", StringComparison.CurrentCulture));
            if (entityProperty == null)
                return default;

            if (!(entityProperty.GetValue(this) is TEntity entity))
                return default;

            var entry = unitOfWork.GetEntityEntry(entity);
            if (entry == null)
                return default;

            var reference2 = entry.Collection(collection);
            var currValue = reference2.CurrentValue;
            if (currValue == null)
                return default;

            var entityList = currValue.ToList();
            if (entityList.Any() != true)
                return default;

            var thisProperty = GetType().GetPublicProperties()
                .FirstOrDefault(x => x.PropertyType == typeof(List<TModel>));
            if (thisProperty == null)
                return default;

            var values = entityList.Select(source => Activator.CreateInstance(typeof(TModel), source, action) as TModel).ToList();
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