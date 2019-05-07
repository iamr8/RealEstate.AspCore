using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class UserViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public User Entity { get; }

        public UserViewModel(User entity, bool includeDeleted, Action<UserViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Username => Entity.Username;
        public Role Role => Entity.Role;
        public string EncryptedPassword => Entity.Password;
        public string Password => Entity.DecryptedPassword;

        public void GetUserItemCategories(bool includeDeleted = false, Action<UserItemCategoryViewModel> action = null)
        {
            UserItemCategories = Entity?.UserItemCategories.Into(includeDeleted, action);
        }

        public void GetUserPropertyCategories(bool includeDeleted = false, Action<UserPropertyCategoryViewModel> action = null)
        {
            UserPropertyCategories = Entity?.UserPropertyCategories.Into(includeDeleted, action);
        }

        public void GetApplicants(bool includeDeleted = false, Action<ApplicantViewModel> action = null)
        {
            Applicants = Entity?.Applicants.Into(includeDeleted, action);
        }

        public void GetBeneficiaries(bool includeDeleted = false, Action<BeneficiaryViewModel> action = null)
        {
            Beneficiaries = Entity?.Beneficiaries.Into(includeDeleted, action);
        }

        public void GetReminders(bool includeDeleted = false, Action<ReminderViewModel> action = null)
        {
            Reminders = Entity?.Reminders.Into(includeDeleted, action);
        }

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = Entity?.Employee.Into(includeDeleted, action);
        }

        public List<UserItemCategoryViewModel> UserItemCategories { get; set; }
        public List<UserPropertyCategoryViewModel> UserPropertyCategories { get; set; }
        public List<ApplicantViewModel> Applicants { get; private set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; private set; }
        public List<ReminderViewModel> Reminders { get; private set; }
        public EmployeeViewModel Employee { get; private set; }
    }
}