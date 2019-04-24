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
        private readonly Deal _entity;

        public DealViewModel(Deal entity, bool includeDeleted, Action<DealViewModel> action = null)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Description => _entity?.Description;
        public double TipPrice => (double)(_entity?.TipPrice ?? 0);
        public double CommissionPrice => (double)(_entity?.CommissionPrice ?? 0);
        public string Barcode => _entity?.Barcode;

        public void GetDealRequest(bool includeDeleted = false, Action<DealRequestViewModel> action = null)
        {
            DealRequest = _entity?.DealRequest.Into(includeDeleted, action);
        }

        public void GetBeneficiaries(bool includeDeleted = false, Action<BeneficiaryViewModel> action = null)
        {
            Beneficiaries = _entity?.Beneficiaries.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted = false, Action<PictureViewModel> action = null)
        {
            Pictures = _entity?.Pictures.Into(includeDeleted, action);
        }

        public void GetReminders(bool includeDeleted = false, Action<ReminderViewModel> action = null)
        {
            Reminders = _entity?.Reminders.Into(includeDeleted, action);
        }

        public DealRequestViewModel DealRequest { get; private set; }
        public List<ReminderViewModel> Reminders { get; private set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; private set; }
        public List<PictureViewModel> Pictures { get; private set; }
    }
}