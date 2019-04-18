using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public bool IsPrivate { get; set; }
        public virtual ICollection<Applicant> Applicants { get; set; }
        public virtual ICollection<Ownership> Ownerships { get; set; }
        public virtual ICollection<Sms> Smses { get; set; }
    }
}