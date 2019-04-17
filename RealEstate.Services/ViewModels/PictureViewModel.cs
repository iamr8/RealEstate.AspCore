using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System;

namespace RealEstate.Services.ViewModels
{
    public class PictureViewModel : BaseLogViewModel<Picture>
    {
        [JsonIgnore]
        private readonly Picture _entity;

        public PictureViewModel(Picture entity, bool includeDeleted, Action<PictureViewModel> action = null) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            _entity = entity;
            Id = entity.Id;
            Logs = entity.GetLogs();
            action?.Invoke(this);
        }

        public string File => _entity.File;
        public string Text => _entity.Text;

        public void GetDeal(bool includeDeleted, Action<DealViewModel> action = null)
        {
            Deal = _entity?.Deal.Into(includeDeleted, action);
        }

        public void GetPayment(bool includeDeleted, Action<PaymentViewModel> action = null)
        {
            Payment = _entity?.Payment.Into(includeDeleted, action);
        }

        public void GetProperty(bool includeDeleted, Action<PropertyViewModel> action = null)
        {
            Property = _entity?.Property.Into(includeDeleted, action);
        }

        public void GetDealPayment(bool includeDeleted, Action<DealPaymentViewModel> action = null)
        {
            DealPayment = _entity?.DealPayment.Into(includeDeleted, action);
        }

        public DealViewModel Deal { get; private set; }
        public PaymentViewModel Payment { get; private set; }
        public PropertyViewModel Property { get; private set; }
        public DealPaymentViewModel DealPayment { get; private set; }
    }
}