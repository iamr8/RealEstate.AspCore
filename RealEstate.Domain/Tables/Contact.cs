using RealEstate.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class Contact : BaseEntity
    {
        public Contact()
        {
            Applicants = new HashSet<Applicant>();
            Ownerships = new HashSet<Ownership>();
            Logs = new HashSet<Log>();
            Smses = new HashSet<Sms>();
        }

        [Required]
        public string MobileNumber { get; set; }

        public virtual ICollection<Applicant> Applicants { get; set; }
        public virtual ICollection<Ownership> Ownerships { get; set; }
        public virtual ICollection<Sms> Smses { get; set; }
    }
}