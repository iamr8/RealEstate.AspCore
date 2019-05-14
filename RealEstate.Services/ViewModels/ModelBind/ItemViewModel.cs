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
            LazyLoadExtension.LazyLoad(() => Entity?.Category.Into<Category, CategoryViewModel>());

        public Lazy<PropertyViewModel> Property =>
            LazyLoadExtension.LazyLoad(() => Entity?.Property.Into<Property, PropertyViewModel>());

        public Lazy<List<ApplicantViewModel>> Applicants =>
            LazyLoadExtension.LazyLoad(() => Entity?.Applicants.Into<Applicant, ApplicantViewModel>());

        public Lazy<List<ItemFeatureViewModel>> ItemFeatures =>
            LazyLoadExtension.LazyLoad(() => Entity?.ItemFeatures.Into<ItemFeature, ItemFeatureViewModel>());

        public Lazy<List<DealRequestViewModel>> DealRequests =>
            LazyLoadExtension.LazyLoad(() => Entity?.DealRequests.Into<DealRequest, DealRequestViewModel>());
    }
}