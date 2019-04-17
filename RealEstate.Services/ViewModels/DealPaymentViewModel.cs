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
        private readonly DealPayment _entity;

        public DealPaymentViewModel(DealPayment entity, bool includeDeleted, Action<DealPaymentViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public decimal Tip => _entity.TipPrice;

        public decimal Commission => _entity.CommissionPrice;

        public DateTime PayDate => _entity.PayDate;

        public string Text => _entity.Text;

        public void GetDeal(bool includeDeleted = false, Action<DealViewModel> action = null)
        {
            Deal = _entity?.Deal.Into(includeDeleted, action);
        }

        public void GetPictures(bool includeDeleted = false, Action<PictureViewModel> action = null)
        {
            Pictures = _entity?.Pictures.Into(includeDeleted, action);
        }

        public DealViewModel Deal { get; private set; }
        public List<PictureViewModel> Pictures { get; private set; }
    }
}