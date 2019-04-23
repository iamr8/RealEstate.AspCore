using RealEstate.Base.Enums;
using RealEstate.Services.Database.Base;
using System.Collections.Generic;

namespace RealEstate.Services.Database.Tables
{
    public class Sms : BaseEntity
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string ReferenceId { get; set; }
        public string Text { get; set; }
        public SmsProvider Provider { get; set; }
        public string StatusJson { get; set; }
        public virtual ICollection<DealRequest> DealRequests { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}