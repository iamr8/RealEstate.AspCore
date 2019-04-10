using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace RealEstate.Services.Extensions.TagHelpers
{
    [HtmlTargetElement("widget-header", Attributes = "title", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class WidgetHeaderTagHelper : TagHelper
    {
        [HtmlAttributeName("title")]
        public string Title { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "header";

            var title = new TagBuilder("h2");

            title.InnerHtml.AppendHtml(Title);
            output.Content.AppendHtml(title);

            var content = output.GetChildContentAsync().Result.GetContent();
            if (!string.IsNullOrEmpty(content))
            {
                var div = new TagBuilder("div");
                div.AddCssClass("other");
                div.InnerHtml.AppendHtml(content);
                output.Content.AppendHtml(div);
            }

            base.Process(context, output);
        }
    }
}