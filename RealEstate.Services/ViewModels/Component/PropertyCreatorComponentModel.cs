using RealEstate.Base;
using RealEstate.Services.ViewModels.Input;

namespace RealEstate.Services.ViewModels.Component
{
    public class PropertyCreatorComponentModel : CreatorComponentModel
    {
        private PropertyInputViewModel _model;
        public PersonCreatorComponentModel PersonCreator { get; set; }
        public string PropertyId { get; set; }

        public PropertyInputViewModel Model
        {
            get => _model ?? new PropertyInputViewModel();
            set => _model = value;
        }

        public HtmlTagViewModel<PropertyInputViewModel> Id => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.Id);

        public HtmlTagViewModel<PropertyInputViewModel> DistrictId => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.DistrictId);
        public HtmlTagViewModel<PropertyInputViewModel> Street => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.Street);
        public HtmlTagViewModel<PropertyInputViewModel> Alley => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.Alley);
        public HtmlTagViewModel<PropertyInputViewModel> Number => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.Number);
        public HtmlTagViewModel<PropertyInputViewModel> BuildingName => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.BuildingName);
        public HtmlTagViewModel<PropertyInputViewModel> Floor => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.Floor);
        public HtmlTagViewModel<PropertyInputViewModel> Flat => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.Flat);
        public HtmlTagViewModel<PropertyInputViewModel> CategoryId => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.CategoryId);
        public HtmlTagViewModel<PropertyInputViewModel> Description => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.Description);
        public HtmlTagViewModel<PropertyInputViewModel> PropertyFeaturesJson => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.PropertyFeaturesJson);
        public HtmlTagViewModel<PropertyInputViewModel> PropertyFeatures => new HtmlTagViewModel<PropertyInputViewModel>(x => x.PropertyFeatures);
        public HtmlTagViewModel<PropertyInputViewModel> PropertyFacilitiesJson => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.PropertyFacilitiesJson);
        public HtmlTagViewModel<PropertyInputViewModel> PropertyFacilities => new HtmlTagViewModel<PropertyInputViewModel>(x => x.PropertyFacilities);
        public HtmlTagViewModel<PropertyInputViewModel> OwnershipsJson => new HtmlTagViewModel<PropertyInputViewModel>(ModelName, x => x.OwnershipsJson);
        public HtmlTagViewModel<PropertyInputViewModel> Ownerships => new HtmlTagViewModel<PropertyInputViewModel>(x => x.Ownerships);
        public string SubmitId => $"{ModelName}_Submit";
        public string FormId => $"{ModelName}_Form";
        public string JsCodeOnSuccess { get; set; }
    }
}