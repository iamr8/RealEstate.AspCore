using RealEstate.Base;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class SmsTemplateViewModel : BaseLogViewModel
    {
        public string Text { get; set; }
        public List<SmsViewModel> Smses { get; set; }
    }
}