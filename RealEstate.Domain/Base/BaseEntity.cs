using RealEstate.Base;
using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;

namespace RealEstate.Domain.Base
{
    public abstract class BaseEntity
    {
        public string Id { get; set; }

        public DateTime DateTime { get; set; }

        // Tracker
        //        public virtual ICollection<Log> Logs { get; set; }

        public LogTypeEnum LastStatus { get; set; }
        public List<LogJsonEntity> Logs { get; set; }
    }
}