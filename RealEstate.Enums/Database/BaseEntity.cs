using System;

namespace RealEstate.Base.Database
{
    public abstract class BaseEntity
    {
        public string Id { get; set; }

        public DateTime DateTime { get; set; }
    }
}