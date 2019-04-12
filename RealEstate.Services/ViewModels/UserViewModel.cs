using JetBrains.Annotations;
using Newtonsoft.Json;
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
        public User Entity { get; private set; }

        [CanBeNull]
        public readonly UserViewModel Instance;

        public UserViewModel()
        {
        }

        public UserViewModel(User entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new UserViewModel
            {
                Entity = entity,

                Role = entity.Role,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Mobile = entity.Mobile,
                EncryptedPassword = entity.Password,
                Username = entity.Username,
                Address = entity.Address,
                Phone = entity.Phone,
                CreationDateTime = entity.DateTime,
                Id = entity.Id,
                Logs = entity.GetLogs()
            };
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