using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using System;

namespace RealEstate.Services.ViewModels.ModelBind
{
    public class PictureViewModel : BaseLogViewModel
    {
        [JsonIgnore]
        public Picture Entity { get; }

        public PictureViewModel(Picture entity, Action<PictureViewModel> act = null)
        {
            if (entity == null)
                return;

            Entity = entity;
            act?.Invoke(this);
        }

        public PictureViewModel()
        {
        }

        public string File => Entity?.File;
        public string Text => Entity?.Text;

        public DealViewModel Deal { get; set; }

        public PaymentViewModel Payment { get; set; }

        public PropertyViewModel Property { get; set; }

        public ReminderViewModel Reminder { get; set; }

        public EmployeeViewModel Employee { get; set; }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}