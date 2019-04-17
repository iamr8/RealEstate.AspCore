using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class ApplicantFeatureViewModel : BaseLogViewModel<ApplicantFeature>
    {
        private string _value;

        [JsonIgnore]
        private readonly ApplicantFeature _entity;

        public ApplicantFeatureViewModel(ApplicantFeature entity, bool includeDeleted, Action<ApplicantFeatureViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Value => _entity.Value.FixCurrency();

        public void GetApplicant(bool includeDeleted = false, Action<ApplicantViewModel> action = null)
        {
            Applicant = _entity?.Applicant.Into(includeDeleted, action);
        }

        public void GetFeature(bool includeDeleted = false, Action<FeatureViewModel> action = null)
        {
            Feature = _entity?.Feature.Into(includeDeleted, action);
        }

        public ApplicantViewModel Applicant { get; private set; }
        public FeatureViewModel Feature { get; private set; }
    }
}