using System;
using RealEstate.Base;

namespace RealEstate.ViewModels
{
    public class NotificationViewModel : BaseViewModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool IsSeen { get; set; }
        public DateTime Sent { get; set; }
    }
}