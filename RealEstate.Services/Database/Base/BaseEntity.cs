using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RealEstate.Services.Database.Base
{
    public abstract class BaseEntity
    {
        public string Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Audit { get; set; }

        [NotMapped]
        public List<LogJsonEntity> Audits
        {
            get => Audit == null ? null : JsonConvert.DeserializeObject<List<LogJsonEntity>>(Audit);
            set => Audit = JsonConvert.SerializeObject(value);
        }

//        [NotMapped]
//        public bool IsDeleted => !string.IsNullOrEmpty(Audit) && Audits?.OrderByDescending(x => x.DateTime).FirstOrDefault()?.Type == LogTypeEnum.Delete;
    }
}