using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class ContactViewModel : BaseLogViewModel<Contact>
    {
        [JsonIgnore]
        private readonly Contact _entity;

        public ContactViewModel(Contact entity, bool includeDeleted, Action<ContactViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Name => _entity.Name;
        public string Phone => _entity.PhoneNumber;
        public string Address => _entity.Address;
        public string Mobile => _entity.MobileNumber;
        public bool IsPrivate => _entity.IsPrivate;

        public void GetOwnerships(bool includeDeleted = false, Action<OwnershipViewModel> action = null)
        {
            Ownerships = _entity?.Ownerships.Into(includeDeleted, action);
        }

        public void GetApplicants(bool includeDeleted = false, Action<ApplicantViewModel> action = null)
        {
            Applicants = _entity?.Applicants.Into(includeDeleted, action);
        }

        public void GetSmses(bool includeDeleted = false, Action<SmsViewModel> action = null)
        {
            Smses = _entity?.Smses.Into(includeDeleted, action);
        }

        public List<OwnershipViewModel> Ownerships { get; private set; }
        public List<SmsViewModel> Smses { get; private set; }
        public List<ApplicantViewModel> Applicants { get; private set; }
    }
}