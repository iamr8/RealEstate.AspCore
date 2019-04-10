using Newtonsoft.Json;
using RealEstate.Base;
using RealEstate.Domain.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Domain.Base
{
    public abstract class BaseEntity
    {
        public string Id { get; set; }

        public DateTime DateTime { get; set; }

        //        Tracker
        public virtual ICollection<Log> Logs { get; set; }

        public string Audit { get; set; }

        [NotMapped]
        public List<LogJsonEntity> Audits
        {
            get => Audit == null ? null : JsonConvert.DeserializeObject<List<LogJsonEntity>>(Audit);
            set => Audit = JsonConvert.SerializeObject(value);
        }
    }
}