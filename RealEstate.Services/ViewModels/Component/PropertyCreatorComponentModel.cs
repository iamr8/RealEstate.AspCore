using RealEstate.Base;
using RealEstate.Services.ViewModels.Input;

namespace RealEstate.Services.ViewModels.Component
{
    public class PropertyCreatorComponentModel : CreatorComponentModel
    {
        public PersonCreatorComponentModel PersonCreator { get; set; }
        public string PropertyId { get; set; }

        public HtmlTagViewModel<OwnershipInputViewModel> Dong => new HtmlTagViewModel<OwnershipInputViewModel>(PersonCreator.ModelName, x => x.Dong);
        public HtmlTagViewModel<PropertyInputViewModel> DistrictId => new HtmlTagViewModel<PropertyInputViewModel>(x => x.DistrictId);
        public HtmlTagViewModel<PropertyInputViewModel> Street => new HtmlTagViewModel<PropertyInputViewModel>(x => x.Street);
        public HtmlTagViewModel<PropertyInputViewModel> Alley => new HtmlTagViewModel<PropertyInputViewModel>(x => x.Alley);
        public HtmlTagViewModel<PropertyInputViewModel> Number => new HtmlTagViewModel<PropertyInputViewModel>(x => x.Number);
        public HtmlTagViewModel<PropertyInputViewModel> BuildingName => new HtmlTagViewModel<PropertyInputViewModel>(x => x.BuildingName);
        public HtmlTagViewModel<PropertyInputViewModel> Floor => new HtmlTagViewModel<PropertyInputViewModel>(x => x.Floor);
        public HtmlTagViewModel<PropertyInputViewModel> Flat => new HtmlTagViewModel<PropertyInputViewModel>(x => x.Flat);
        public HtmlTagViewModel<PropertyInputViewModel> CategoryId => new HtmlTagViewModel<PropertyInputViewModel>(x => x.CategoryId);
        public HtmlTagViewModel<PropertyInputViewModel> Description => new HtmlTagViewModel<PropertyInputViewModel>(x => x.Description);
        public HtmlTagViewModel<PropertyInputViewModel> PropertyFeaturesJson => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.PropertyFeaturesJson);
        public HtmlTagViewModel<PropertyInputViewModel> PropertyFeatures => new HtmlTagViewModel<PropertyInputViewModel>(x => x.PropertyFeatures);
        public HtmlTagViewModel<PropertyInputViewModel> PropertyFacilitiesJson => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.PropertyFacilitiesJson);
        public HtmlTagViewModel<PropertyInputViewModel> PropertyFacilities => new HtmlTagViewModel<PropertyInputViewModel>(x => x.PropertyFacilities);
        public HtmlTagViewModel<PropertyInputViewModel> OwnershipsJson => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.OwnershipsJson);
        public HtmlTagViewModel<PropertyInputViewModel> Ownerships => new HtmlTagViewModel<PropertyInputViewModel>(x => x.Ownerships);
    }
}