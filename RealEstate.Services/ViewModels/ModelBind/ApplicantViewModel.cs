using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ApplicantViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Applicant Entity { get; }

        public ApplicantViewModel(Applicant entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Description => Entity?.Description;
        public ApplicantTypeEnum Type => Entity?.Type ?? ApplicantTypeEnum.Applicant;

        public Lazy<CustomerViewModel> Customer => LazyLoadExtension.LazyLoad(() => Entity?.Customer.Map<Customer, CustomerViewModel>());
        public Lazy<UserViewModel> User => LazyLoadExtension.LazyLoad(() => Entity?.User.Map<User, UserViewModel>());
        public Lazy<ItemViewModel> Item => LazyLoadExtension.LazyLoad(() => Entity?.Item.Map<Item, ItemViewModel>());
        public Lazy<List<ApplicantFeatureViewModel>> ApplicantFeatures => LazyLoadExtension.LazyLoad(() => Entity?.ApplicantFeatures.Map<ApplicantFeature, ApplicantFeatureViewModel>());
    }
}