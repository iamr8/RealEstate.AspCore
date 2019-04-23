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
        private readonly Item _entity;

        public ItemViewModel(Item entity, bool includeDeleted, Action<ItemViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Description => _entity?.Description;
        public DealStatusEnum LastState => _entity?.DealRequests.OrderDescendingByCreationDateTime().FirstOrDefault()?.Status ?? DealStatusEnum.Rejected;

        public void GetCategory(bool includeDeleted = false, Action<CategoryViewModel> action = null)
        {
            Category = _entity?.Category.Into(includeDeleted, action);
        }

        public void GetProperty(bool includeDeleted = false, Action<PropertyViewModel> action = null)
        {
            Property = _entity?.Property.Into(includeDeleted, action);
        }

        public void GetItemFeatures(bool includeDeleted = false, Action<ItemFeatureViewModel> action = null)
        {
            ItemFeatures = _entity?.ItemFeatures.Into(includeDeleted, action);
        }

        public void GetDealRequests(bool includeDeleted = false, Action<DealRequestViewModel> action = null)
        {
            DealRequests = _entity?.DealRequests?.Into(includeDeleted, action);
        }

        public void GetApplicants(bool includeDeleted, Action<ApplicantViewModel> action = null)
        {
            Applicants = _entity?.Applicants.Into(includeDeleted, action).ShowBasedOn(x => x.Customer);
        }

        public List<ApplicantViewModel> Applicants { get; private set; }

        public CategoryViewModel Category { get; private set; }
        public PropertyViewModel Property { get; private set; }
        public List<ItemFeatureViewModel> ItemFeatures { get; private set; }
        public List<DealRequestViewModel> DealRequests { get; private set; }
    }
}