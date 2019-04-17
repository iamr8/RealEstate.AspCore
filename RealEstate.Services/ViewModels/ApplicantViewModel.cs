using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class ApplicantViewModel : BaseLogViewModel<Applicant>
    {
        [JsonIgnore]
        private readonly Applicant _entity;

        public ApplicantViewModel(Applicant entity, bool includeDeleted, Action<ApplicantViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Description => _entity.Description;
        public ApplicantTypeEnum Type => _entity.Type;

        public void GetContact(bool includeDeleted = false, Action<ContactViewModel> action = null)
        {
            Contact = _entity?.Contact.Into(includeDeleted, action);
        }

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public void GetDeal(bool includeDeleted = false, Action<DealViewModel> action = null)
        {
            Deal = _entity?.Deal.Into(includeDeleted, action);
        }

        public void GetApplicantFeatures(bool includeDeleted = false, Action<ApplicantFeatureViewModel> action = null)
        {
            ApplicantFeatures = _entity?.ApplicantFeatures.Into(includeDeleted, action);
        }

        public ContactViewModel Contact { get; private set; }
        public UserViewModel User { get; private set; }
        public DealViewModel Deal { get; private set; }
        public List<ApplicantFeatureViewModel> ApplicantFeatures { get; private set; }
    }
}