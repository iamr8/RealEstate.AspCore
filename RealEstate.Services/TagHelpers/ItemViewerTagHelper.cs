using System;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;

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
        public bool AsRow { get; set; } = false;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";

            if (AsRow)
                output.AddClass("row", HtmlEncoder.Default);

            output.AddClass("grid", HtmlEncoder.Default);
            output.TagMode = TagMode.StartTagAndEndTag;

            if (Model?.Model == null)
                goto NO_ITEM;

            var pagination = Model.Model;
            var type = pagination.GetType();

            if (!type.IsSubclassOf(typeof(PaginationViewModel)))
                throw new Exception("Model that given to item-viewer is not typeof PaginationViewModel");

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            if (properties?.Any() != true)
                throw new Exception("Model that given to item-viewer is not typeof PaginationViewModel");

            PaginationViewModel pag;
            var currentPageProp = properties.FirstOrDefault(x => x.Name.Equals(nameof(pag.CurrentPage), StringComparison.CurrentCulture));
            var rowCountProp = properties.FirstOrDefault(x => x.Name.Equals(nameof(pag.Rows), StringComparison.CurrentCulture));
            var pagesProp = properties.FirstOrDefault(x => x.Name.Equals(nameof(pag.Pages), StringComparison.CurrentCulture));
            var itemsProp = properties.FirstOrDefault(x => x.Name.Equals("Items", StringComparison.CurrentCulture));

            if (currentPageProp == null || rowCountProp == null || pagesProp == null || itemsProp == null)
                throw new Exception("Model that given to item-viewer is not typeof PaginationViewModel");

            int currentPage;
            int rowCount;
            int pages;
            try
            {
                currentPage = (int)currentPageProp.GetValue(pagination);
                rowCount = (int)rowCountProp.GetValue(pagination);
                pages = (int)pagesProp.GetValue(pagination);
            }
            catch
            {
                throw new Exception("Model that given to item-viewer is not typeof PaginationViewModel");
            }

            output.Attributes.Add("data-rows", rowCount);

            if (rowCount > 0)
            {
                var content = (await output.GetChildContentAsync()).GetContent();
                output.Content.AppendHtml(content);
                return;
            }

            NO_ITEM:
            var message = new TagBuilder("h5");
            message.InnerHtml.AppendHtml(_localizer[SharedResource.NoItemToShow]);
            if (!AsRow)
                output.AddClass("row", HtmlEncoder.Default);
            output.AddClass("justify-content-center", HtmlEncoder.Default);
            output.AddClass("align-items-center", HtmlEncoder.Default);
            output.Content.AppendHtml(message);
        }
    }
}