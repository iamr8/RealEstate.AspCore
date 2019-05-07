using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class ApplicantFeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public ApplicantFeature Entity { get; }

        public ApplicantFeatureViewModel(ApplicantFeature entity, bool includeDeleted, Action<ApplicantFeatureViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Value => Entity.Value.FixCurrency();

        public void GetApplicant(bool includeDeleted = false, Action<ApplicantViewModel> action = null)
        {
            Applicant = Entity?.Applicant.Into(includeDeleted, action).ShowBasedOn(x => x.Customer);
        }

        public void GetFeature(bool includeDeleted = false, Action<FeatureViewModel> action = null)
        {
            Feature = Entity?.Feature.Into(includeDeleted, action);
        }

        public ApplicantViewModel Applicant { get; private set; }
        public FeatureViewModel Feature { get; private set; }
    }
}