using RealEstate.Base;
using RealEstate.Base.Enums;

namespace RealEstate.ViewModels
{
    public class SmsViewModel : BaseViewModel
    {
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