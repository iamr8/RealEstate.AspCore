using RealEstate.Domain.Base;
using System.Collections.Generic;

namespace RealEstate.Domain.Tables
{
    public class SmsTemplate : BaseEntity
    {
        public SmsTemplate()
        {
            Logs = new HashSet<Log>();
        }

        public string Text { get; set; }
        public virtual ICollection<Sms> Smses { get; set; }
    }
}