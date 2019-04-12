using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class ApplicantFeatureViewModel : BaseLogViewModel<ApplicantFeature>
    {
        private string _value;

        [JsonIgnore]
        public ApplicantFeature Entity { get; private set; }

        [CanBeNull]
        public readonly ApplicantFeatureViewModel Instance;

        public ApplicantFeatureViewModel()
        {
        }

        public ApplicantFeatureViewModel(ApplicantFeature entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new ApplicantFeatureViewModel
            {
                Entity = entity,
                Id = entity.Id,
                Value = entity.Value,
                Logs = entity.GetLogs()
            };
        }

        public string Value
        {
            get => _value.FixCurrency();
            set => _value = value;
        }

        public FeatureViewModel Feature { get; set; }
    }
}