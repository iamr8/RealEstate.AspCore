using RealEstate.Base;
using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class UserViewModel : BaseLogViewModel
    {
        public string Username { get; set; }
        public string Id { get; set; }
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
    }
}