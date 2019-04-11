using RealEstate.Services.BaseLog;
using RealEstate.Services.Database.Tables;

namespace RealEstate.Services.ViewModels
{
    public class PropertyFacilityViewModel : BaseLogViewModel<PropertyFacility>
    {
        public PropertyFacilityViewModel(PropertyFacility entity) : base(entity)
        {
            Id = entity.Id;
        }

        public PropertyFacilityViewModel()
        {
        }

        public FacilityViewModel Facility { get; set; }
    }
}