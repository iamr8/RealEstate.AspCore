using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class PaymentViewModel : BaseLogViewModel<Payment>
    {
        [JsonIgnore]
        public Payment Entity { get; private set; }

        [CanBeNull]
        public readonly PaymentViewModel Instance;

        public PaymentViewModel(Payment entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new PaymentViewModel
            {
                Entity = entity,
                Value = entity.Value,
                Id = entity.Id,
                Text = entity.Text,
                Type = entity.Type,
                Logs = entity.GetLogs()
            };
        }

        public PaymentViewModel()
        {
        }

        public double Value { get; set; }

        public string Text { get; set; }
        public PaymentTypeEnum Type { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
    }
}