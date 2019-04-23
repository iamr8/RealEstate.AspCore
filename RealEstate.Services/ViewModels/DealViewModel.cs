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

        public void GetDealRequest(bool includeDeleted = false, Action<DealRequestViewModel> action = null)
        {
            DealRequest = _entity?.DealRequest.Into(includeDeleted, action);
        }

        public void GetDealPayments(bool includeDeleted = false, Action<DealPaymentViewModel> action = null)
        {
            DealPayments = _entity?.DealPayments.Into(includeDeleted, action).ShowBasedOn(x => x.Deal);
        }

        public void GetBeneficiaries(bool includeDeleted = false, Action<BeneficiaryViewModel> action = null)
        {
            Beneficiaries = _entity?.Beneficiaries.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted = false, Action<PictureViewModel> action = null)
        {
            Pictures = _entity?.Pictures.Into(includeDeleted, action);
        }

        public void GetChecks(bool includeDeleted = false, Action<CheckViewModel> action = null)
        {
            Checks = _entity?.Checks.Into(includeDeleted, action);
        }

        public DealRequestViewModel DealRequest { get; private set; }
        public List<DealPaymentViewModel> DealPayments { get; private set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; private set; }
        public List<PictureViewModel> Pictures { get; private set; }
        public List<CheckViewModel> Checks { get; private set; }
    }
}