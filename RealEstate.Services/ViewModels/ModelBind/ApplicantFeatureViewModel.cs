using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class ApplicantFeatureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public ApplicantFeature Entity { get; }

        public ApplicantFeatureViewModel(ApplicantFeature entity, Action<ApplicantFeatureViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Value => Entity.Value.FixCurrency();

        public ApplicantViewModel Applicant { get; set; }
        public FeatureViewModel Feature { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}