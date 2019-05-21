using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using RealEstate.Resources;
using System.Text.Encodings.Web;

namespace RealEstate.Services.TagHelpers
{
    [HtmlTargetElement("submit", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SubmitTagHelper : TagHelper
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SubmitTagHelper(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public bool FullWidth { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "input";
            output.TagMode = TagMode.SelfClosing;
            output.AddClass("btn-success", HtmlEncoder.Default);

            if (FullWidth)
                output.AddClass("btn-block", HtmlEncoder.Default);

            output.Attributes.Add("value", _localizer[SharedResource.Submit]);
            output.Attributes.Add("type", "submit");
        }
    }
}