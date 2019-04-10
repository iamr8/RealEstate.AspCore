using System;
using RealEstate.Base;

namespace RealEstate.Services.ViewModels
{
    public class NotificationViewModel : BaseViewModel
    {
        public string Text { get; set; }
        public bool IsSeen { get; set; }
        public DateTime Sent { get; set; }
    }
}