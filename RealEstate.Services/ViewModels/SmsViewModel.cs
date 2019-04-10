using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.ViewModels
{
    public class SmsViewModel : BaseLogViewModel<Sms>
    {
        public SmsViewModel()
        {
        }

        public SmsViewModel(Sms model) : base(model)
        {
            if (model == null)
                return;

            Sender = model.Sender;
            Id = model.Id;
            Receiver = model.Receiver;
            ReferenceId = model.ReferenceId;
            Text = model.Text;
            Provider = model.Provider;
            StatusJson = model.StatusJson;
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