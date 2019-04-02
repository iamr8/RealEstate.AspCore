using RealEstate.Base.Enums;
using RealEstate.Domain.Base;

namespace RealEstate.Domain.Tables
{
    public class Sms : BaseEntity
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string ReferenceId { get; set; }
        public string Text { get; set; }
        public SmsProvider Provider { get; set; }
        public string StatusJson { get; set; }
        public string ContactId { get; set; }
        public string SmsTemplateId { get; set; }
        public string UserId { get; set; }
        public virtual SmsTemplate SmsTemplate { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual User User { get; set; }
    }
}