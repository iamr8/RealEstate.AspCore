using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class SmsTemplateViewModel : BaseLogViewModel<SmsTemplate>
    {
        public SmsTemplateViewModel()
        {
        }

        public SmsTemplateViewModel(SmsTemplate model, bool showDeleted) : base(model)
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