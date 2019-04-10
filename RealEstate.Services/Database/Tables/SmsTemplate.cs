using RealEstate.Services.Database.Base;
using System.Collections.Generic;

namespace RealEstate.Services.Database.Tables
{
    public class SmsTemplate : BaseEntity
    {
        public string Text { get; set; }
        public virtual ICollection<Sms> Smses { get; set; }
    }
}