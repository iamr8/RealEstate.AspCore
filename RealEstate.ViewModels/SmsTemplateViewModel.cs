using RealEstate.Base;
using RealEstate.Domain.Tables;
using System.Collections.Generic;

namespace RealEstate.ViewModels
{
    public class SmsTemplateViewModel : BaseLogViewModel<SmsTemplate>
    {
        public SmsTemplateViewModel()
        {
        }

        public SmsTemplateViewModel(SmsTemplate model) : base(model)
        {
            if (model == null)
                return;

            Text = model.Text;
            Id = model.Id;
        }

        public string Text { get; set; }
        public List<SmsViewModel> Smses { get; set; }
    }
}