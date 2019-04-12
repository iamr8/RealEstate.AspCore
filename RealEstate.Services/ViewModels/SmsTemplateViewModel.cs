using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using System.Collections.Generic;

namespace RealEstate.Services.ViewModels
{
    public class SmsTemplateViewModel : BaseLogViewModel<SmsTemplate>
    {
        [JsonIgnore]
        public SmsTemplate Entity { get; private set; }

        [CanBeNull]
        public readonly SmsTemplateViewModel Instance;

        public SmsTemplateViewModel()
        {
        }

        public SmsTemplateViewModel(SmsTemplate entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new SmsTemplateViewModel
            {
                Entity = entity,
                Text = entity.Text,
                Id = entity.Id,
                Logs = entity.GetLogs()
            };
        }

        public string Text { get; set; }
        public List<SmsViewModel> Smses { get; set; }
    }
}