using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base.Enums;

namespace RealEstate.Services.Database.Tables
{
    public class User : BaseEntity
    {
        //public User()
        //{
        //    Beneficiaries = new HashSet<Beneficiary>();
        //    Applicants = new HashSet<Applicant>();
        //    UserItemCategories = new HashSet<UserItemCategory>();
        //    UserPropertyCategories = new HashSet<UserPropertyCategory>();
        //    Payments = new HashSet<Payment>();
        //    Smses = new HashSet<Sms>();
        //    Permissions = new HashSet<Permission>();
        //    FixedSalaries = new HashSet<FixedSalary>();
        //    Reminders = new HashSet<Reminder>();
        //}

        [Required]
        public Role Role { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        public virtual ICollection<Applicant> Applicants { get; set; }
        public virtual ICollection<UserItemCategory> UserItemCategories { get; set; }
        public virtual ICollection<UserPropertyCategory> UserPropertyCategories { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }
        public virtual ICollection<Check> Checks { get; set; }
    }
}