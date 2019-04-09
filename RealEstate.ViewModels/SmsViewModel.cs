using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Domain.Tables;

namespace RealEstate.ViewModels
{
    public class SmsViewModel : BaseViewModel
    {
        private readonly Sms _model;

        public SmsViewModel()
        {
        }

        public SmsViewModel(Sms model)
        {
            if (model == null)
                return;

            _model = model;
            Sender = _model.Sender;
            Id = _model.Id;
            Receiver = _model.Receiver;
            ReferenceId = _model.ReferenceId;
            Text = _model.Text;
            Provider = _model.Provider;
            StatusJson = _model.StatusJson;
        }

        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string ReferenceId { get; set; }
        public string Text { get; set; }
        public SmsProvider Provider { get; set; }
        public string StatusJson { get; set; }
        public UserViewModel User { get; set; }
        public ContactViewModel Contact { get; set; }
        public SmsTemplateViewModel SmsTemplate { get; set; }
    }
}