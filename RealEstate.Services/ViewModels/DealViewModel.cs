using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class DealViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Deal Entity { get; }

        public DealViewModel(Deal entity, bool includeDeleted, Action<DealViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Entity = entity;
            action?.Invoke(this);
        }

        public string Description => Entity?.Description;
        public double TipPrice => (double)(Entity?.TipPrice ?? 0);
        public double CommissionPrice => (double)(Entity?.CommissionPrice ?? 0);
        public string Barcode => Entity?.Barcode;

        public void GetDealRequest(bool includeDeleted = false, Action<DealRequestViewModel> action = null)
        {
            DealRequest = Entity?.DealRequest.Into(includeDeleted, action);
        }

        public void GetBeneficiaries(bool includeDeleted = false, Action<BeneficiaryViewModel> action = null)
        {
            Beneficiaries = Entity?.Beneficiaries.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted = false, Action<PictureViewModel> action = null)
        {
            Pictures = Entity?.Pictures.Into(includeDeleted, action);
        }

        public void GetReminders(bool includeDeleted = false, Action<ReminderViewModel> action = null)
        {
            Reminders = Entity?.Reminders.Into(includeDeleted, action);
        }

        public DealRequestViewModel DealRequest { get; private set; }
        public List<ReminderViewModel> Reminders { get; private set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; private set; }
        public List<PictureViewModel> Pictures { get; private set; }
    }
}