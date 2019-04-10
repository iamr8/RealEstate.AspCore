using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class UserViewModel : BaseLogViewModel<User>
    {
        public UserViewModel()
        {
        }

        public UserViewModel(User entity) : base(entity)
        {
            Role = Entity.Role;
            FirstName = Entity.FirstName;
            LastName = Entity.LastName;
            Mobile = Entity.Mobile;
            EncryptedPassword = Entity.Password;
            Username = Entity.Username;
            Address = Entity.Address;
            Phone = Entity.Phone;
            CreationDateTime = Entity.DateTime;
            Id = Entity.Id;
        }

        public string Username { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public Role Role { get; set; }

        public string EncryptedPassword { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public DateTime CreationDateTime { get; set; }
        public List<UserItemCategoryViewModel> ItemCategories { get; set; }
        public List<UserPropertyCategoryViewModel> PropertyCategories { get; set; }
        public List<ApplicantViewModel> Applicants { get; set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }
        public List<PaymentViewModel> Payments { get; set; }
        public List<SmsViewModel> Smses { get; set; }
        public List<PermissionViewModel> Permissions { get; set; }
        public List<FixedSalaryViewModel> FixedSalaries { get; set; }
    }
}