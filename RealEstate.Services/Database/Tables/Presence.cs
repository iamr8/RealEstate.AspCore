using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Presence : BaseEntity
    {
        [Required]
        public PresenseStatusEnum Status { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public override string ToString()
        {
            return $"{Status.GetDisplayName()} در تاریخ {Date.GregorianToPersian(true)} {Hour}:{Minute}";
        }
    }
}