using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ItemViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Item Entity { get; }

        public ItemViewModel(Item entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Description => Entity?.Description;

        public DealStatusEnum LastState() => DealRequests.LazyLoadLast()?.Status ?? DealStatusEnum.Rejected;

        public Lazy<CategoryViewModel> Category =>
            LazyLoadExtension.LazyLoad(() => Entity?.Category.Map<Category, CategoryViewModel>());

        public Lazy<PropertyViewModel> Property =>
            LazyLoadExtension.LazyLoad(() => Entity?.Property.Map<Property, PropertyViewModel>());

        public Lazy<List<ApplicantViewModel>> Applicants =>
            LazyLoadExtension.LazyLoad(() => Entity?.Applicants.Map<Applicant, ApplicantViewModel>());

        public Lazy<List<ItemFeatureViewModel>> ItemFeatures =>
            LazyLoadExtension.LazyLoad(() => Entity?.ItemFeatures.Map<ItemFeature, ItemFeatureViewModel>());

        public Lazy<List<DealRequestViewModel>> DealRequests =>
            LazyLoadExtension.LazyLoad(() => Entity?.DealRequests.Map<DealRequest, DealRequestViewModel>());
    }
}