using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Database;

namespace RealEstate.Domain.Tables
{
    public class Contact : BaseEntity
    {
        public Contact()
        {
            Applicants = new HashSet<Applicant>();
            Owners = new HashSet<Owner>();
        }

        public string Description { get; set; }

        [Required]
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        public string Address { get; set; }
        public virtual ICollection<Applicant> Applicants { get; set; }
        public virtual ICollection<Owner> Owners { get; set; }
    }
}