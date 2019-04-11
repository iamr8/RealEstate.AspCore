namespace RealEstate.Services.ViewModels.Component
{
    public class PropertySelectorComplexComponentModel
    {
        public string ParentModelName { get; set; }
        public PropertyCreatorComponentModel PropertyCreator { get; set; }
        public PropertySelectorComponentModel PropertySelector { get; set; }
    }
}