using Microsoft.AspNetCore.Mvc.Rendering;
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
            var contentContext = await output.GetChildContentAsync();
            var content = contentContext.GetContent().Replace("\r\n", "").Trim();

            var span = new TagBuilder("span");
            span.InnerHtml.AppendHtml(content);

            var closeButton = new TagBuilder("a");
            closeButton.Attributes.Add("href", "#");
            closeButton.AddCssClass("close");
            closeButton.Attributes.Add("style", "float: left");
            closeButton.InnerHtml.AppendHtml("&times;");

            output.AddClass("status-message", HtmlEncoder.Default);
            output.AddClass("text-right", HtmlEncoder.Default);
            output.Attributes.Add("id", "pageSTATUS");
            if (string.IsNullOrEmpty(content))
                output.AddClass("hidden", HtmlEncoder.Default);
            else
                output.Content.AppendHtml(span).AppendHtml(closeButton);

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";
        }
    }
}