using RealEstate.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.ViewModels
{
    public class DistrictViewModel : BaseLogViewModel<District>
    {
        public DistrictViewModel(District model) : base(model)
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