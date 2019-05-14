using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class UserViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public User Entity { get; }

        public UserViewModel(User entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Username => Entity?.Username;
        public Role Role => Entity?.Role ?? Role.User;
        public string EncryptedPassword => Entity?.Password;
        public string Password => Entity?.DecryptedPassword;

        public Lazy<List<UserItemCategoryViewModel>> UserItemCategories =>
            LazyLoadExtension.LazyLoad(() => Entity?.UserItemCategories.Into<UserItemCategory, UserItemCategoryViewModel>());

        public Lazy<List<UserPropertyCategoryViewModel>> UserPropertyCategories =>
            LazyLoadExtension.LazyLoad(() => Entity?.UserPropertyCategories.Into<UserPropertyCategory, UserPropertyCategoryViewModel>());

        public Lazy<List<ApplicantViewModel>> Applicants => LazyLoadExtension.LazyLoad(() => Entity?.Applicants.Into<Applicant, ApplicantViewModel>());
        public Lazy<List<BeneficiaryViewModel>> Beneficiaries => LazyLoadExtension.LazyLoad(() => Entity?.Beneficiaries.Into<Beneficiary, BeneficiaryViewModel>());
        public Lazy<List<ReminderViewModel>> Reminders => LazyLoadExtension.LazyLoad(() => Entity?.Reminders.Into<Reminder, ReminderViewModel>());
        public Lazy<EmployeeViewModel> Employee => LazyLoadExtension.LazyLoad(() => Entity?.Employee.Into<Employee, EmployeeViewModel>());
    }
}