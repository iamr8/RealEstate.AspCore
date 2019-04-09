using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Domain.Tables;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        private readonly Payment _model;

        public PaymentViewModel(Payment model)
        {
            if (model == null)
                return;

            _model = model;
            Value = _model.Value;
            Id = _model.Id;
            Text = _model.Text;
            Type = _model.Type;
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