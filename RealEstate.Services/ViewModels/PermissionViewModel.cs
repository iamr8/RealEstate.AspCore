using RealEstate.Base.Enums;
using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.ViewModels
{
    public class PermissionViewModel : BaseLogViewModel<Permission>
    {
        public PermissionViewModel()
        {
        }

        public PermissionViewModel(Permission model) : base(model)
        {
            if (model == null)
                return;

            Key = model.Key;
            Id = model.Id;
            Type = model.Type;
        }

        public string Key { get; set; }
        public PermissionTypeEnum Type { get; set; }
    }
}