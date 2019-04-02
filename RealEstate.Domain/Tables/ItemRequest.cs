using RealEstate.Domain.Base;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class ItemRequest : BaseEntity
    {
        public ItemRequest()
        {
            Applicants = new HashSet<Applicant>();
            Logs = new HashSet<Log>();
        }

        public string Description { get; set; }
        public bool IsReject { get; set; }
        public string ItemId { get; set; }
        public virtual Item Item { get; set; }
        public virtual Deal Deal { get; set; }
        public virtual ICollection<Applicant> Applicants { get; set; }
    }
}