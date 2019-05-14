using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class DealViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Deal Entity { get; }

        public DealViewModel(Deal entity)
        {
            if (entity == null)
                return;

            Entity = entity;
        }

        public string Description => Entity?.Description;
        public double TipPrice => (double)(Entity?.TipPrice ?? 0);
        public double CommissionPrice => (double)(Entity?.CommissionPrice ?? 0);
        public string Barcode => Entity?.Barcode;

        public Lazy<DealRequestViewModel> DealRequest => LazyLoadExtension.LazyLoad(() => Entity?.DealRequest.Into<DealRequest, DealRequestViewModel>());
        public Lazy<List<ReminderViewModel>> Reminders => LazyLoadExtension.LazyLoad(() => Entity?.Reminders.Into<Reminder, ReminderViewModel>());
        public Lazy<List<BeneficiaryViewModel>> Beneficiaries => LazyLoadExtension.LazyLoad(() => Entity?.Beneficiaries.Into<Beneficiary, BeneficiaryViewModel>());
        public Lazy<List<PictureViewModel>> Pictures => LazyLoadExtension.LazyLoad(() => Entity?.Pictures.Into<Picture, PictureViewModel>());
    }
}