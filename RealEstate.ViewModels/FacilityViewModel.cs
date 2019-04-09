using RealEstate.Base;
using RealEstate.Domain.Tables;

namespace RealEstate.ViewModels
{
    public class FacilityViewModel : BaseLogViewModel<Facility>
    {
        public FacilityViewModel(Facility model) : base(model)
        {
            if (model == null)
                return;

            Name = model.Name;
            Properties = model.PropertyFacilities.Count;
            Id = model.Id;
        }

        public FacilityViewModel()
        {
        }

        public string Name { get; set; }

        public int Properties { get; set; }
    }
}