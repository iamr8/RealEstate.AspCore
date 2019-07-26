using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using RealEstate.Services.Database.Base;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.Database.Tables
{
    public class PropertyOwnership : BaseEntity
    {
        public string PropertyId { get; set; }

        public virtual Property Property { get; set; }
        public virtual ICollection<Ownership> Ownerships { get; set; }

        [NotMapped]
        public Ownership CurrentOwnership => Ownerships?.OrderDescendingByCreationDateTime().FirstOrDefault();
    }
}