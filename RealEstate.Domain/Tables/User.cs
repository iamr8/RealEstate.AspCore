using RealEstate.Base.Enums;
using RealEstate.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Domain.Tables
{
    public class User : BaseEntity
    {
        public User()
        {
            Beneficiaries = new HashSet<Beneficiary>();
            Applicants = new HashSet<Applicant>();
            UserItemCategories = new HashSet<UserItemCategory>();
            UserPropertyCategories = new HashSet<UserPropertyCategory>();
            Payments = new HashSet<Payment>();
            Logs = new HashSet<Log>();
            Smses = new HashSet<Sms>();
            Permissions = new HashSet<Permission>();
        }

        [Required]
        public Role Role { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Mobile { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Password { get; set; }

        public double FixedSalary { get; set; }
        public DateTime DateOfPay { get; set; }
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        public virtual ICollection<Applicant> Applicants { get; set; }
        public virtual ICollection<UserItemCategory> UserItemCategories { get; set; }
        public virtual ICollection<UserPropertyCategory> UserPropertyCategories { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Sms> Smses { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}