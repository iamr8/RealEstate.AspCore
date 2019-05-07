using RealEstate.Base;
using RealEstate.Base.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RealEstate.Services.Database.Base
{
    public abstract class BaseEntity
    {
        public string Id { get; set; }

        public string Audit { get; set; }

        [NotMapped]
        public List<LogJsonEntity> Audits
        {
            get => Audit.JsonGetAccessor<LogJsonEntity>();
            set => Audit = value.JsonSetAccessor();
        }

        [NotMapped]
        public LogJsonEntity LastAudit => Audits?.OrderByDescending(x => x.DateTime).FirstOrDefault();

        [NotMapped]
        public bool IsDeleted => !string.IsNullOrEmpty(Audit) && Audits?.OrderByDescending(x => x.DateTime).FirstOrDefault()?.Type == LogTypeEnum.Delete;
    }
}