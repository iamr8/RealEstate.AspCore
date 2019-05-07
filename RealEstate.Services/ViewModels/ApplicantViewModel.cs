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
        public Applicant Entity { get; }

        public ApplicantViewModel(Applicant entity, bool includeDeleted, Action<ApplicantViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Description => Entity?.Description;
        public ApplicantTypeEnum Type => Entity?.Type ?? ApplicantTypeEnum.Applicant;

        public void GetCustomer(bool includeDeleted = false, Action<CustomerViewModel> action = null)
        {
            Customer = Entity?.Customer.Into(includeDeleted, action);
        }

        public void GetUser(bool includeDeleted = false, Action<UserViewModel> action = null)
        {
            User = Entity?.User.Into(includeDeleted, action);
        }

        public void GetItem(bool includeDeleted = false, Action<ItemViewModel> action = null)
        {
            Item = Entity?.Item.Into(includeDeleted, action).ShowBasedOn(x => x.Property);
        }

        public void GetApplicantFeatures(bool includeDeleted = false, Action<ApplicantFeatureViewModel> action = null)
        {
            ApplicantFeatures = Entity?.ApplicantFeatures.Into(includeDeleted, action).ShowBasedOn(x => x.Feature);
        }

        public CustomerViewModel Customer { get; private set; }
        public UserViewModel User { get; private set; }
        public ItemViewModel Item { get; private set; }
        public List<ApplicantFeatureViewModel> ApplicantFeatures { get; private set; }
    }
}