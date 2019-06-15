using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ItemViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Item Entity { get; }

        public ItemViewModel(Item entity, Action<ItemViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public ItemViewModel(Item entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Description => Entity?.Description?.Replace("قابل مذاکره", "", StringComparison.CurrentCultureIgnoreCase);
        public bool IsNegotiable => Entity?.Description?.Contains("قابل مذاکره", StringComparison.CurrentCultureIgnoreCase) == true;
        public DealStatusEnum LastState => DealRequests?.OrderDescendingByCreationDateTime().FirstOrDefault()?.Status ?? DealStatusEnum.Rejected;

        public CategoryViewModel Category { get; set; }

        public PropertyViewModel Property { get; set; }

        public List<ApplicantViewModel> Applicants { get; set; }

        public List<ItemFeatureViewModel> ItemFeatures { get; set; }
        public List<DealRequestViewModel> DealRequests { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}