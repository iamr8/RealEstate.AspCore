using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class DealPaymentViewModel : BaseLogViewModel<DealPayment>
    {
        [JsonIgnore]
        public DealPayment Entity { get; private set; }

        [CanBeNull]
        public readonly DealPaymentViewModel Instance;

        public DealPaymentViewModel(DealPayment entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new DealPaymentViewModel
            {
                Entity = entity,
                Id = entity.Id,
                Logs = entity.GetLogs(),
                Text = entity.Text,
                PayDate = entity.PayDate,
                Commission = entity.CommissionPrice,
                Tip = entity.TipPrice
            };
        }

        public DealPaymentViewModel()
        {
        }

        public decimal Tip { get; set; }

        public decimal Commission { get; set; }

        public DateTime PayDate { get; set; }

        public string Text { get; set; }

        public List<PictureViewModel> Pictures { get; set; }
    }
}