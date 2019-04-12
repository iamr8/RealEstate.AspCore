using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class ContactViewModel : BaseLogViewModel<Contact>
    {
        [JsonIgnore]
        public Contact Entity { get; private set; }

        [CanBeNull]
        public readonly ContactViewModel Instance;

        public ContactViewModel()
        {
        }

        public ContactViewModel(Contact entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new ContactViewModel
            {
                Entity = entity,
                Id = entity.Id,
                Mobile = entity.MobileNumber,
                Phone = entity.PhoneNumber,
                Address = entity.Address,
                IsPrivate = entity.IsPrivate,
                Name = entity.Name,
                Logs = entity.GetLogs()
            };
        }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public bool IsPrivate { get; set; }
        public List<SmsViewModel> Smses { get; set; }
        public List<ApplicantViewModel> Applicants { get; set; }
        public List<OwnershipViewModel> Ownerships { get; set; }
    }
}