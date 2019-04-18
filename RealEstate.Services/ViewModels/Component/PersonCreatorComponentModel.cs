using RealEstate.Base;
using RealEstate.Services.ViewModels.Input;

namespace RealEstate.Services.ViewModels.Component
{
    public class PersonCreatorComponentModel : CreatorComponentModel
    {
        public HtmlTagViewModel<CustomerInputViewModel> Name => new HtmlTagViewModel<CustomerInputViewModel>(ModelName, x => x.Name);
        public HtmlTagViewModel<CustomerInputViewModel> Mobile => new HtmlTagViewModel<CustomerInputViewModel>(ModelName, x => x.Mobile);
        public HtmlTagViewModel<CustomerInputViewModel> Phone => new HtmlTagViewModel<CustomerInputViewModel>(ModelName, x => x.Phone);
        public HtmlTagViewModel<CustomerInputViewModel> Address => new HtmlTagViewModel<CustomerInputViewModel>(ModelName, x => x.Address);
        public HtmlTagViewModel<CustomerInputViewModel> Description => new HtmlTagViewModel<CustomerInputViewModel>(ModelName, x => x.Description);
        public string SubmitId => $"{ModelName}_Submit";
        public string FormId => $"{ModelName}_Form";
        public string JsCodeOnSuccess { get; set; }
    }
}