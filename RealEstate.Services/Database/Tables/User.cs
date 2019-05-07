using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Services.Database.Tables
{
    public class User : BaseEntity
    {
        [Required]
        public Role Role { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [NotMapped]
        public string DecryptedPassword => Password.Cipher(CryptologyExtension.CypherMode.Decryption);

        public string EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; }
        public virtual ICollection<Applicant> Applicants { get; set; }
        public virtual ICollection<UserItemCategory> UserItemCategories { get; set; }
        public virtual ICollection<UserPropertyCategory> UserPropertyCategories { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }
    }
}