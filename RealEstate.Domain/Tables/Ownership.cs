using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class Ownership : BaseEntity
    {
        public Ownership()
        {
            Logs = new HashSet<Log>();
        }

        [DefaultValue(6)]
        public int Dong { get; set; }

        [Required]
        public string Name { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public string Description { get; set; }
        public string PropertyOwnershipId { get; set; }
        public string ContactId { get; set; }
        public virtual PropertyOwnership PropertyOwnership { get; set; }
        public virtual Contact Contact { get; set; }
    }
}