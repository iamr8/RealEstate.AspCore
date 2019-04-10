using RealEstate.Services.Database.Base;
using System.Collections.Generic;

namespace RealEstate.Services.Database.Tables
{
    public class ItemRequest : BaseEntity
    {
        public ItemRequest()
        {
            Applicants = new HashSet<Applicant>();
        }

        public string Description { get; set; }
        public bool IsReject { get; set; }
        public string ItemId { get; set; }
        public virtual Item Item { get; set; }
        public virtual Deal Deal { get; set; }
        public virtual ICollection<Applicant> Applicants { get; set; }
    }
}