using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels
{
    public class ItemViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Item Entity { get; }

        public ItemViewModel(Item entity, bool includeDeleted, Action<ItemViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Description => Entity?.Description;
        public DealStatusEnum LastState => Entity?.DealRequests.OrderDescendingByCreationDateTime().FirstOrDefault()?.Status ?? DealStatusEnum.Rejected;

        public void GetCategory(bool includeDeleted = false, Action<CategoryViewModel> action = null)
        {
            Category = Entity?.Category.Into(includeDeleted, action);
        }

        public void GetProperty(bool includeDeleted = false, Action<PropertyViewModel> action = null)
        {
            Property = Entity?.Property.Into(includeDeleted, action);
        }

        public void GetItemFeatures(bool includeDeleted = false, Action<ItemFeatureViewModel> action = null)
        {
            ItemFeatures = Entity?.ItemFeatures.Into(includeDeleted, action);
        }

        public void GetDealRequests(bool includeDeleted = false, Action<DealRequestViewModel> action = null)
        {
            DealRequests = Entity?.DealRequests?.Into(includeDeleted, action);
        }

        public void GetApplicants(bool includeDeleted, Action<ApplicantViewModel> action = null)
        {
            Applicants = Entity?.Applicants.Into(includeDeleted, action).ShowBasedOn(x => x.Customer);
        }

        public List<ApplicantViewModel> Applicants { get; private set; }

        public CategoryViewModel Category { get; private set; }
        public PropertyViewModel Property { get; private set; }
        public List<ItemFeatureViewModel> ItemFeatures { get; private set; }
        public List<DealRequestViewModel> DealRequests { get; private set; }
    }
}