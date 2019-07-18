using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RealEstate.Services.TagHelpers
{
    [HtmlTargetElement("status", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class StatusTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var contentContext = await output.GetChildContentAsync().ConfigureAwait(false);
            var content = contentContext.GetContent().Replace("\r\n", "").Trim();

            output.AddClass("status-message", HtmlEncoder.Default);
            output.AddClass("text-right", HtmlEncoder.Default);

            if (string.IsNullOrEmpty(content))
                output.AddClass("hidden", HtmlEncoder.Default);
            else
                output.Content.AppendHtml(content);

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
        }
    }
}