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
    public class UserViewModel : BaseLogViewModel<User>
    {
        [JsonIgnore]
        private readonly User _entity;

        public UserViewModel(User entity, bool includeDeleted, Action<UserViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            Username = entity.Username;
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            Mobile = entity.Mobile;
            Role = entity.Role;
            EncryptedPassword = entity.Password;
            Address = entity.Address;
            Phone = entity.Phone;
            CreationDateTime = entity.DateTime;
            action?.Invoke(this);
        }

        public UserViewModel()
        {
        }

        public string Username { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public Role Role { get; set; }

        public string EncryptedPassword { get; set; }
        public string Password => EncryptedPassword.Cipher(CryptologyExtension.CypherMode.Decryption);
        public string Address { get; set; }
        public string Phone { get; set; }

        public DateTime CreationDateTime { get; set; }

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

        public void GetPayments(bool includeDeleted = false, Action<PaymentViewModel> action = null)
        {
            Payments = _entity?.Payments.Into(includeDeleted, action);
        }

        public void GetSmses(bool includeDeleted = false, Action<SmsViewModel> action = null)
        {
            Smses = _entity?.Smses.Into(includeDeleted, action);
        }

        public void GetPermissions(bool includeDeleted = false, Action<PermissionViewModel> action = null)
        {
            Permissions = _entity?.Permissions.Into(includeDeleted, action);
        }

        public void GetFixedSalaries(bool includeDeleted = false, Action<FixedSalaryViewModel> action = null)
        {
            FixedSalaries = _entity?.FixedSalaries.Into(includeDeleted, action);
        }

        public void GetReminders(bool includeDeleted = false, Action<ReminderViewModel> action = null)
        {
            Reminders = _entity?.Reminders.Into(includeDeleted, action);
        }

        public List<UserItemCategoryViewModel> UserItemCategories { get; set; }
        public List<UserPropertyCategoryViewModel> UserPropertyCategories { get; set; }
        public List<ApplicantViewModel> Applicants { get; private set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; private set; }
        public List<PaymentViewModel> Payments { get; private set; }
        public List<SmsViewModel> Smses { get; private set; }
        public List<PermissionViewModel> Permissions { get; private set; }
        public List<FixedSalaryViewModel> FixedSalaries { get; private set; }
        public List<ReminderViewModel> Reminders { get; private set; }
    }
}