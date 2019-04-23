using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RealEstate.Services.Database.Tables
{
    public class Customer : BaseEntity
    {
        public Customer()
        {
            Applicants = new HashSet<Applicant>();
            Ownerships = new HashSet<Ownership>();
            Smses = new HashSet<Sms>();
        }

        [Required]
        public string MobileNumber { get; set; }

        [Required]
        public string Name { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        [NotMapped]
        public bool IsPublic => Applicants.Count == 0
                                || Ownerships.Count >= 0
                                || Applicants.Any(x => x.Item.DealRequests.Any(c => c.DealId != null));

        public virtual ICollection<Applicant> Applicants { get; set; }

        public virtual ICollection<Ownership> Ownerships { get; set; }
        public virtual ICollection<Sms> Smses { get; set; }
    }
}