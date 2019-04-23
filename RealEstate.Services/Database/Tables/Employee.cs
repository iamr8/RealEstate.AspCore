using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Database.Tables
{
    public class Employee : BaseEntity
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Mobile { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<EmployeeStatus> EmployeeStatuses { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<FixedSalary> FixedSalaries { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<ManagementPercent> ManagementPercents { get; set; }
        public virtual ICollection<Insurance> Insurances { get; set; }
        public virtual ICollection<Leave> Leaves { get; set; }
        public virtual ICollection<Presence> Presences { get; set; }
        public virtual ICollection<EmployeeDivision> EmployeeDivisions { get; set; }
        public virtual ICollection<Sms> Smses { get; set; }
    }
}