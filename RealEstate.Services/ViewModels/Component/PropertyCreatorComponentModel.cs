using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstate.Base;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using System.Collections.Generic;
using System.Linq;

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

        public List<CustomerViewModel> OwnershipList { get; set; }
        public List<FacilityViewModel> FacilityList { get; set; }
        public List<FeatureViewModel> FeaturesList { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public List<DistrictViewModel> Districts { get; set; }
        public string OwnershipSelectorTitle { get; set; }
        public string FacilitySelectorTitle { get; set; }
        public string FeaturesSelectorTitle { get; set; }

        public JsonSelectorComponentModel FeatureSelector => new JsonSelectorComponentModel
        {
            IdProperty = PropertyExtensions.GetJsonProperty<FeatureJsonValueViewModel>(x => x.Id),
            NameProperty = PropertyExtensions.GetJsonProperty<FeatureJsonValueViewModel>(x => x.Name),
            ValueProperty = PropertyExtensions.GetJsonProperty<FeatureJsonValueViewModel>(x => x.Value),
            ModelName = ModelName,
            ItemName = PropertyFeatures.Name,
            Json = nameof(PropertyFeaturesJson),
            Title = FeaturesSelectorTitle,
            ScriptKey = "propertyFeaturesScript",
            SelectListItems = FeaturesList?.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id,
            }).SelectFirstItem(),
        };

        public JsonSelectorComponentModel FacilitySelector => new JsonSelectorComponentModel
        {
            IdProperty = PropertyExtensions.GetJsonProperty<FacilityJsonViewModel>(x => x.Id),
            NameProperty = PropertyExtensions.GetJsonProperty<FacilityJsonViewModel>(x => x.Name),
            ModelName = ModelName,
            ItemName = PropertyFacilities.Name,
            Json = nameof(PropertyFacilitiesJson),
            Title = FacilitySelectorTitle,
            ScriptKey = "propertyFacilitiesScript",
            SelectListItems = FacilityList?.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id,
            }).SelectFirstItem(),
        };

        public JsonSelectorComponentModel OwnershipSelector => new JsonSelectorComponentModel
        {
            IdProperty = PropertyExtensions.GetJsonProperty<OwnershipJsonViewModel>(x => x.CustomerId),
            NameProperty = PropertyExtensions.GetJsonProperty<OwnershipJsonViewModel>(x => x.Name),
            ValueProperty = PropertyExtensions.GetJsonProperty<OwnershipJsonViewModel>(x => x.Dong),
            ModelName = ModelName,
            ItemName = Ownerships.Name,
            Json = nameof(Model.OwnershipsJson),
            Title = OwnershipSelectorTitle,
            ScriptKey = "propertyOwnersScript",
            SelectListItems = OwnershipList?.Select(x => new SelectListItem
            {
                Text = $"{x.Name} • {x.Mobile}",
                Value = x.Id,
            }).SelectFirstItem(),
        };
    }
}