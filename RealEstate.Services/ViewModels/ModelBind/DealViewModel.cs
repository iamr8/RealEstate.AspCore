using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class DealViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Deal Entity { get; }

        public DealViewModel(Deal entity, Action<DealViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public string Description => Entity?.Description;
        public double TipPrice => (double)(Entity?.TipPrice ?? 0);
        public double CommissionPrice => (double)(Entity?.CommissionPrice ?? 0);
        public string Barcode => Entity?.Barcode;

        public DealRequestViewModel DealRequest { get; set; }
        public List<ReminderViewModel> Reminders { get; set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
    }
}