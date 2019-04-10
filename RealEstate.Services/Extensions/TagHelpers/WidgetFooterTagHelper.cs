using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace RealEstate.Services.Extensions.TagHelpers
{
    public class ParentChildContext
    {
        public IHtmlContent TextContext { get; set; }
        public IHtmlContent ButtonsContext { get; set; }
    }

    [RestrictChildren("widget-footer-text", "widget-footer-button")]
    [HtmlTargetElement("widget-footer")]
    public class WidgetFooterTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var parentContext = new ParentChildContext();
            context.Items.Add(typeof(ParentChildContext), parentContext);

            await output.GetChildContentAsync().ConfigureAwait(false);
            output.TagName = "footer";

            output.Content.AppendHtml(parentContext.TextContext ?? new WidgetFooterTextTagHelper().Div);
            output.Content.AppendHtml(parentContext.ButtonsContext);
        }
    }

    [HtmlTargetElement("widget-footer-text", ParentTag = "widget-footer")]
    public class WidgetFooterTextTagHelper : TagHelper
    {
        public readonly TagBuilder Div;

        public WidgetFooterTextTagHelper()
        {
            var div = new TagBuilder("div");
            div.AddCssClass("messages");
            Div = div;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var parentChild = context.Items[typeof(ParentChildContext)] as ParentChildContext;

            var content = output.GetChildContentAsync().Result;

            Div.InnerHtml.AppendHtml(content);
            parentChild.TextContext = Div;
            output.SuppressOutput();
        }
    }

    [HtmlTargetElement("widget-footer-button", ParentTag = "widget-footer")]
    public class WidgetFooterButtonTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var parentChild = context.Items[typeof(ParentChildContext)] as ParentChildContext;

            var content = output.GetChildContentAsync().Result;

            var div = new TagBuilder("div");
            div.AddCssClass("buttons");
            div.InnerHtml.AppendHtml(content);
            parentChild.ButtonsContext = div;
            output.SuppressOutput();
        }
    }
}