using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RealEstate.Base;
using RealEstate.Base.Database;
using RealEstate.Base.Enums;

namespace RealEstate.Domain.Tables
{
    public class Log : BaseEntity
    {
        public Log()
        {
            NotificationSeeners = new HashSet<NotificationSeener>();
            NotificationRecipients = new HashSet<NotificationRecipient>();
        }

        public TrackTypeEnum Type { get; set; }

        [Required]
        public string CreatorId { get; set; }

        public string Text { get; set; }
        public string EntityId { get; set; }
        public virtual ICollection<NotificationSeener> NotificationSeeners { get; set; }
        public virtual ICollection<NotificationRecipient> NotificationRecipients { get; set; }
    }
}