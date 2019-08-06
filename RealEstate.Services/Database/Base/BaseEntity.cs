using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;

namespace RealEstate.Services.Database.Base
{
    public abstract class BaseEntity
    {
        public string Id { get; set; }

        public string Audit { get; set; }

        [NotMapped]
        public List<LogJsonEntity> Audits
        {
            get => JsonExtensions.Deserialize<List<LogJsonEntity>>(Audit);
            set => Audit = value.Serialize();
        }

        [NotMapped]
        public LogJsonEntity LastAudit => Audits?.OrderByDescending(x => x.DateTime).FirstOrDefault();

        [NotMapped]
        public bool IsDeleted => !string.IsNullOrEmpty(Audit) && Audits?.OrderByDescending(x => x.DateTime).FirstOrDefault()?.Type == LogTypeEnum.Delete;

        public TModel Map<TModel>(Action<TModel> action = null) where TModel : BaseLogViewModel
        {
            var result = Activator.CreateInstance(typeof(TModel), this, action) as TModel;
            return result;
        }
    }
}