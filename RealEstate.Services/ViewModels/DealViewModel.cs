using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class DealViewModel : BaseLogViewModel<Deal>
    {
        [JsonIgnore]
        public Deal Entity { get; private set; }

        [CanBeNull]
        public readonly DealViewModel Instance;

        public DealViewModel(Deal entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new DealViewModel
            {
                Entity = entity,
                Description = entity.Description,
                Id = entity.Id,
                Logs = entity.GetLogs(),
                Status = entity.Status
            };
        }

        public DealViewModel()
        {
        }

        public string Description { get; set; }
        public DealStatusEnum Status { get; set; }
        public ItemViewModel Item { get; set; }
        public List<ApplicantViewModel> Applicants { get; set; }
        public List<DealPaymentViewModel> DealPayments { get; set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
    }
}