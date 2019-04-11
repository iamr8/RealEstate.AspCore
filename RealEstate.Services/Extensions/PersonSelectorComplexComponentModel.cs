using RealEstate.Services.ViewModels.Component;

namespace RealEstate.Services.Extensions
{
    public class PersonSelectorComplexComponentModel
    {
        public string ParentModelName { get; set; }

        public SelectorComponentModel PersonSelector { get; set; }
        public CreatorComponentModel PersonCreator { get; set; }
        public string ResetId { get; set; }
    }
}