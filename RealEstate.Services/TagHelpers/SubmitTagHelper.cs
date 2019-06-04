using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using RealEstate.Base.Enums;
using RealEstate.Resources;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RealEstate.Services.TagHelpers
{
    [HtmlTargetElement("submit", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SubmitTagHelper : TagHelper
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        public bool Ajax { get; set; }
        public BsButtonTypeEnum Type { get; set; } = BsButtonTypeEnum.Success;

        public SubmitTagHelper(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public bool FullWidth { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = (await output.GetChildContentAsync().ConfigureAwait(false)).GetContent(HtmlEncoder.Default);

            output.TagName = Ajax ? "button" : "input";
            output.TagMode = Ajax ? TagMode.StartTagAndEndTag : TagMode.SelfClosing;
            output.AddClass("btn", HtmlEncoder.Default);
            output.AddClass($"btn-{Type.ToString().ToLower()}", HtmlEncoder.Default);

            if (FullWidth)
                output.AddClass("btn-block", HtmlEncoder.Default);

            if (Ajax)
                output.AddClass("btn-flex", HtmlEncoder.Default);

            var value = string.IsNullOrEmpty(content) ? _localizer[SharedResource.Submit] : content;
            if (Ajax)
                output.Content.AppendHtml(value);
            else
                output.Attributes.Add("value", value);

            output.Attributes.Add("type", Ajax ? "button" : "submit");
        }
    }
}