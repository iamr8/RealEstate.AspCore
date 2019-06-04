using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class UserViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public User Entity { get; }

        public UserViewModel(User entity, Action<UserViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Username => Entity?.Username;
        public Role Role => Entity?.Role ?? Role.User;
        public string EncryptedPassword => Entity?.Password;
        public string Password => Entity?.DecryptedPassword;

        public List<UserItemCategoryViewModel> UserItemCategories { get; set; }

        public List<UserPropertyCategoryViewModel> UserPropertyCategories { get; set; }

        public List<ApplicantViewModel> Applicants { get; set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }
        public List<ReminderViewModel> Reminders { get; set; }
        public EmployeeViewModel Employee { get; set; }

        public override string ToString()
        {
            return Entity?.ToString();
        }
    }
}