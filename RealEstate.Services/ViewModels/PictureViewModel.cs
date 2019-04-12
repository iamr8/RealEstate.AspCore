using JetBrains.Annotations;
using Newtonsoft.Json;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;

namespace RealEstate.Services.ViewModels
{
    public class PictureViewModel : BaseLogViewModel<Picture>
    {
        [JsonIgnore]
        public Picture Entity { get; private set; }

        [CanBeNull]
        public readonly PictureViewModel Instance;

        public PictureViewModel(Picture entity, bool includeDeleted) : base(entity)
        {
            if (entity == null || (entity.IsDeleted && !includeDeleted))
                return;

            Instance = new PictureViewModel
            {
                Entity = entity,
                File = entity.File,
                Id = entity.Id,
                Text = entity.Text,
                Logs = entity.GetLogs()
            };
        }

        public PictureViewModel()
        {
        }

        public string File { get; set; }
        public string Text { get; set; }
    }
}