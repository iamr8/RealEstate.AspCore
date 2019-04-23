using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class ApplicantViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        private readonly Applicant _entity;

        public ApplicantViewModel(Applicant entity, bool includeDeleted, Action<ApplicantViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Description => _entity?.Description;
        public ApplicantTypeEnum Type => _entity?.Type ?? ApplicantTypeEnum.Applicant;

        public void GetCustomer(bool includeDeleted = false, Action<CustomerViewModel> action = null)
        {
            Customer = _entity?.Customer.Into(includeDeleted, action);
        }

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = _entity?.User.Into(includeDeleted, action);
        }

        public void GetItem(bool includeDeleted = false, Action<ItemViewModel> action = null)
        {
            Item = _entity?.Item.Into(includeDeleted, action).ShowBasedOn(x => x.Property);
        }

        public void GetApplicantFeatures(bool includeDeleted = false, Action<ApplicantFeatureViewModel> action = null)
        {
            ApplicantFeatures = _entity?.ApplicantFeatures.Into(includeDeleted, action).ShowBasedOn(x => x.Feature);
        }

        public CustomerViewModel Customer { get; private set; }
        public UserViewModel User { get; private set; }
        public ItemViewModel Item { get; private set; }
        public List<ApplicantFeatureViewModel> ApplicantFeatures { get; private set; }
    }
}