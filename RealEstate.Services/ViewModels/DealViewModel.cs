using Newtonsoft.Json;
using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class DealViewModel : BaseLogViewModel<Deal>
    {
        [JsonIgnore]
        private readonly Deal _entity;

        public DealViewModel(Deal entity, bool includeDeleted, Action<DealViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string Description => _entity.Description;
        public DealStatusEnum Status => _entity.Status;

        public void GetItem(bool includeDeleted, Action<ItemViewModel> action = null)
        {
            Item = _entity?.Item.Into(includeDeleted, action);
        }

        public void GetApplicants(bool includeDeleted, Action<ApplicantViewModel> action = null)
        {
            Applicants = _entity?.Applicants.Into(includeDeleted, action);
        }

        public void GetDealPayments(bool includeDeleted, Action<DealPaymentViewModel> action = null)
        {
            DealPayments = _entity?.DealPayments.Into(includeDeleted, action);
        }

        public void GetBeneficiaries(bool includeDeleted, Action<BeneficiaryViewModel> action = null)
        {
            Beneficiaries = _entity?.Beneficiaries.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted, Action<PictureViewModel> action = null)
        {
            Pictures = _entity?.Pictures.Into(includeDeleted, action);
        }

        public ItemViewModel Item { get; private set; }
        public List<ApplicantViewModel> Applicants { get; private set; }
        public List<DealPaymentViewModel> DealPayments { get; private set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; private set; }
        public List<PictureViewModel> Pictures { get; private set; }
    }
}