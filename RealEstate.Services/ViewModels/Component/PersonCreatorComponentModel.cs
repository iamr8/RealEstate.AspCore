using RealEstate.Base;
using RealEstate.Services.ViewModels.Input;

namespace RealEstate.Services.ViewModels.Component
{
    public class PersonCreatorComponentModel : CreatorComponentModel
    {
        public HtmlTagViewModel<ContactInputViewModel> Name => new HtmlTagViewModel<ContactInputViewModel>(ModelName, x => x.Name);
        public HtmlTagViewModel<ContactInputViewModel> Mobile => new HtmlTagViewModel<ContactInputViewModel>(ModelName, x => x.Mobile);
        public HtmlTagViewModel<ContactInputViewModel> Phone => new HtmlTagViewModel<ContactInputViewModel>(ModelName, x => x.Phone);
        public HtmlTagViewModel<ContactInputViewModel> Address => new HtmlTagViewModel<ContactInputViewModel>(ModelName, x => x.Address);
        public HtmlTagViewModel<ContactInputViewModel> Description => new HtmlTagViewModel<ContactInputViewModel>(ModelName, x => x.Description);
        public string SubmitId => $"{ModelName}_Submit";
        public string FormId => $"{ModelName}_Form";
        public string JsCodeOnSuccess { get; set; }
    }
}