using Newtonsoft.Json;
using RealEstate.Base;
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
        private readonly User _entity;

        public UserViewModel(User entity, bool includeDeleted, Action<UserViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Username => _entity.Username;
        public Role Role => _entity.Role;
        public string EncryptedPassword => _entity.Password;
        public string Password => EncryptedPassword.Cipher(CryptologyExtension.CypherMode.Decryption);

        public void GetUserItemCategories(bool includeDeleted = false, Action<UserItemCategoryViewModel> action = null)
        {
            UserItemCategories = _entity?.UserItemCategories.Into(includeDeleted, action);
        }

        public void GetUserPropertyCategories(bool includeDeleted = false, Action<UserPropertyCategoryViewModel> action = null)
        {
            UserPropertyCategories = _entity?.UserPropertyCategories.Into(includeDeleted, action);
        }

        public void GetApplicants(bool includeDeleted = false, Action<ApplicantViewModel> action = null)
        {
            Applicants = _entity?.Applicants.Into(includeDeleted, action);
        }

        public void GetBeneficiaries(bool includeDeleted = false, Action<BeneficiaryViewModel> action = null)
        {
            Beneficiaries = _entity?.Beneficiaries.Into(includeDeleted, action);
        }

        public void GetReminders(bool includeDeleted = false, Action<ReminderViewModel> action = null)
        {
            Reminders = _entity?.Reminders.Into(includeDeleted, action);
        }

        public void GetEmployee(bool includeDeleted = false, Action<EmployeeViewModel> action = null)
        {
            Employee = _entity?.Employee.Into(includeDeleted, action);
        }

        public List<UserItemCategoryViewModel> UserItemCategories { get; set; }
        public List<UserPropertyCategoryViewModel> UserPropertyCategories { get; set; }
        public List<ApplicantViewModel> Applicants { get; private set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; private set; }
        public List<ReminderViewModel> Reminders { get; private set; }
        public EmployeeViewModel Employee { get; private set; }
    }
}