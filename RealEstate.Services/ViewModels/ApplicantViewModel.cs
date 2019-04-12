using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Extensions;
using System.Collections.Generic;
using Applicant = RealEstate.Services.Database.Tables.Applicant;

namespace RealEstate.Services.ViewModels
{
    public class ApplicantViewModel : BaseLogViewModel<Applicant>
    {
        [JsonIgnore]
        public Applicant Entity { get; private set; }

        [CanBeNull]
        public readonly ApplicantViewModel Instance;

        public ApplicantViewModel(Applicant entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new ApplicantViewModel
            {
                Entity = entity,
                Type = entity.Type,
                Description = entity.Description,
                Id = entity.Id,
                Logs = entity.GetLogs()
            };
        }

        public ApplicantViewModel()
        {
        }

        public string Description { get; set; }
        public ApplicantTypeEnum Type { get; set; }
        public ContactViewModel Contact { get; set; }
        public UserViewModel User { get; set; }
        public List<ApplicantFeatureViewModel> ApplicantFeatures { get; set; }
    }
}