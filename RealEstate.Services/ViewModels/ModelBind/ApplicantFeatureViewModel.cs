using System;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ApplicantFeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public ApplicantFeature Entity { get; }

        public ApplicantFeatureViewModel(ApplicantFeature entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Value => Entity.Value.FixCurrency();

        public Lazy<ApplicantViewModel> Applicant => LazyLoadExtension.LazyLoad(() => Entity?.Applicant.Into<Applicant, ApplicantViewModel>());
        public Lazy<FeatureViewModel> Feature => LazyLoadExtension.LazyLoad(() => Entity?.Feature.Into<Feature, FeatureViewModel>());
    }
}