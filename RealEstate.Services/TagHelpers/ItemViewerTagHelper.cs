using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Localization;
using RealEstate.Resources;
using RealEstate.Services.BaseLog;
using System;
using System.Collections;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RealEstate.Services.TagHelpers
{
    [HtmlTargetElement("item-viewer", Attributes = "model", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ItemViewerTagHelper : TagHelper
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ItemViewerTagHelper(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public ModelExpression Model { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Model?.Model == null)
            {
                var message = new TagBuilder("h5");
                message.InnerHtml.AppendHtml(_localizer[SharedResource.NoItemToShow]);

                output.TagName = "div";
                output.AddClass("row", HtmlEncoder.Default);
                output.AddClass("justify-content-center", HtmlEncoder.Default);
                output.AddClass("align-items-center", HtmlEncoder.Default);
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(message);
                return;
            }

            var items = Model.Model;
            var type = items.GetType();
            var genericTypes = type.GenericTypeArguments;
            if (genericTypes.Length == 0)
                throw new Exception("Can't find any generic class");

            var isBaseTypeOk = genericTypes[0].IsSubclassOf(typeof(BaseLogViewModel));
            if (!isBaseTypeOk)
                throw new Exception("Model type should be member of BaseLogViewModel");

            if (items is IList itemList)
            {
                if (itemList?.Any() == true)
                {
                    var content = (await output.GetChildContentAsync().ConfigureAwait(false)).GetContent();
                    output.SuppressOutput();
                    output.PostContent.AppendHtml(content);
                    return;
                }
                var message = new TagBuilder("h5");
                message.InnerHtml.AppendHtml(_localizer[SharedResource.NoItemToShow]);

                output.TagName = "div";
                output.AddClass("row", HtmlEncoder.Default);
                output.AddClass("justify-content-center", HtmlEncoder.Default);
                output.AddClass("align-items-center", HtmlEncoder.Default);
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(message);
                return;
            }

            output.SuppressOutput();
            return;
        }
    }
}