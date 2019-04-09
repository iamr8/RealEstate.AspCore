using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Domain.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class UserViewModel : BaseLogViewModel<User>
    {
        public UserViewModel()
        {
        }

        public UserViewModel(User entity) : base(entity)
        {
            if (entity == null)
                return;

            Role = Entity.Role;
            FirstName = Entity.FirstName;
            LastName = Entity.LastName;
            Mobile = Entity.Mobile;
            EncryptedPassword = Entity.Password;
            Username = Entity.Username;
            Address = Entity.Address;
            Phone = Entity.Phone;
            CreationDateTime = Entity.DateTime;
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
        public List<CategoryViewModel> ItemCategories { get; set; }
        public List<CategoryViewModel> PropertyCategories { get; set; }
        public List<ApplicantViewModel> Applicants { get; set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }
        public List<PaymentViewModel> Payments { get; set; }
        public List<SmsViewModel> Smses { get; set; }
        public List<PermissionViewModel> Permissions { get; set; }
    }
}