using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using RealEstate.Resources;
using RestSharp.Extensions;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RealEstate.Services.TagHelpers
{
    [HtmlTargetElement("search-pane", Attributes = "is-under-condition", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SearchPaneTagHelper : TagHelper
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public SearchPaneTagHelper(
            IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public bool IsUnderCondition { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = (await output.GetChildContentAsync().ConfigureAwait(false)).GetContent();
            const string cardName = "search";

            var url = ViewContext.RouteData.Values.FirstOrDefault(x => x.Key.Equals("page")).Value;
            var pageNo = string.Empty;

            var queryString = ViewContext.HttpContext.Request.QueryString.Value;
            queryString = queryString.Substring(queryString.StartsWith("?") ? 1 : 0);
            if (!string.IsNullOrEmpty(queryString))
                pageNo = queryString.Split("&").FirstOrDefault(x => x.Split("=")[0].Equals("pageNo", StringComparison.CurrentCultureIgnoreCase)).UrlDecode();

            var cardTitle = new TagBuilder("a");
            cardTitle.AddCssClass("card-link");
            cardTitle.Attributes.Add("data-toggle", "collapse");
            cardTitle.Attributes.Add("href", $"#{cardName}");
            cardTitle.InnerHtml.AppendHtml(_localizer[SharedResource.Search]);

            var cardCustomTitle = new TagBuilder("a");
            cardCustomTitle.AddCssClass("card-link");
            cardCustomTitle.AddCssClass("clearSearch");
            cardCustomTitle.Attributes.Add("href", $"{url}{(string.IsNullOrEmpty(pageNo) ? "" : "?" + pageNo)}");
            cardCustomTitle.InnerHtml.AppendHtml(_localizer[SharedResource.ClearSearch]);

            var cardHeader = new TagBuilder("div");
            cardHeader.AddCssClass("card-header");
            cardHeader.InnerHtml.AppendHtml(cardTitle);
            cardHeader.InnerHtml.AppendHtml(cardCustomTitle);

            var cardBody = new TagBuilder("div");
            cardBody.AddCssClass("card-body");
            cardBody.InnerHtml.AppendHtml(content);

            var collapse = new TagBuilder("div");
            collapse.Attributes.Add("id", cardName);
            collapse.AddCssClass("collapse");
            if (IsUnderCondition)
                collapse.AddCssClass("show");
            collapse.InnerHtml.AppendHtml(cardBody);

            var card = new TagBuilder("div");
            card.AddCssClass("card");
            card.InnerHtml.AppendHtml(cardHeader);
            card.InnerHtml.AppendHtml(collapse);

            var col = new TagBuilder("div");
            col.AddCssClass("col-12");
            col.InnerHtml.AppendHtml(card);

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.AddClass("row", HtmlEncoder.Default);
            output.AddClass("justify-content-center", HtmlEncoder.Default);
            output.AddClass("align-items-center", HtmlEncoder.Default);
            output.AddClass("search", HtmlEncoder.Default);
            output.Content.AppendHtml(col);
        }
    }
}