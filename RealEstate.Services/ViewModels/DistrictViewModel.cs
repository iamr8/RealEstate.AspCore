using RealEstate.Domain.Tables;
using RealEstate.Services.BaseLog;

namespace RealEstate.Services.ViewModels
{
    public class DistrictViewModel : BaseLogViewModel<District>
    {
        public DistrictViewModel(District model, bool showDeleted) : base(model)
        {
            if (model == null)
                return;

            Id = model.Id;
            Name = model.Name;
            Properties = model.Properties.Count;
        }

        public DistrictViewModel()
        {
        }

        public string Name { get; set; }

        public int Properties { get; set; }
    }
}