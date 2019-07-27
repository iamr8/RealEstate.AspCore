using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using RealEstate.Base;
using RealEstate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RealEstate.Services.TagHelpers
{
    [RestrictChildren("li")]
    [HtmlTargetElement("search-nav", Attributes = "rows, searchModel", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SearchNavBarTagHelper : TagHelper
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IUrlHelperFactory _urlHelperFactory;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("searchModel")]
        public ModelExpression SearchModel { get; set; }

        [HtmlAttributeName("rows")]
        public int Rows { get; set; }

        public SearchNavBarTagHelper(
            IStringLocalizer<SharedResource> localizer,
            IUrlHelperFactory urlHelperFactory)
        {
            _localizer = localizer;
            _urlHelperFactory = urlHelperFactory;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            bool? isTriggered = false;
            int? pageNo = 0;
            var searchOptions = new Dictionary<string, string>();

            if (SearchModel != null)
            {
                BaseSearchModel baseSearch;
                var modeltype = SearchModel.Model.GetType();
                if (modeltype.BaseType == typeof(BaseSearchModel))
                {
                    var properties = modeltype.GetPublicProperties();
                    isTriggered = (bool?)properties.Where(x => x.Name.Equals(nameof(baseSearch.IsTriggered), StringComparison.CurrentCulture))
                        .Select(x => x.GetValue(SearchModel.Model)).FirstOrDefault();
                    pageNo = (int?)properties.Where(x => x.Name.Equals(nameof(baseSearch.PageNo), StringComparison.CurrentCulture))
                        .Select(x => x.GetValue(SearchModel.Model)).FirstOrDefault();
                    searchOptions = properties.Where(x => x.Name.Equals(nameof(baseSearch.Conditions), StringComparison.CurrentCulture))
                        .Select(x => x.GetValue(SearchModel.Model)).FirstOrDefault() as Dictionary<string, string>;
                }
            }

            var navbarLeft = new TagBuilder("ul");
            navbarLeft.AddCssClass("navbar-nav");

            var content = (await output.GetChildContentAsync()).GetContent();
            var search = new TagBuilder("li");
            search.AddCssClass("nav-item");

            var searchModalButton = new TagBuilder("a");
            searchModalButton.AddCssClass("nav-link");
            searchModalButton.AddCssClass("btn-sm");
            searchModalButton.AddCssClass("btn-secondary");

            if (isTriggered != null && isTriggered == true)
                searchModalButton.AddCssClass("active");

            searchModalButton.Attributes.Add("type", "button");
            searchModalButton.Attributes.Add("data-toggle", "modal");
            searchModalButton.Attributes.Add("data-target", "#searchModal");

            var searchIcon = new TagBuilder("i");
            searchIcon.AddCssClass("fa");
            searchIcon.AddCssClass("fa-search");

            searchModalButton.InnerHtml.AppendHtml(searchIcon).AppendHtml(_localizer[SharedResource.SearchFilters]);
            search.InnerHtml.AppendHtml(searchModalButton);
            navbarLeft.InnerHtml.AppendHtml(search).AppendHtml(content);

            var navbarRight = new TagBuilder("ul");
            navbarRight.AddCssClass("navbar-nav");
            navbarRight.AddCssClass("mr-sm-auto");

            #region ClearSearch

            if (isTriggered != null && isTriggered == true)
            {
                var clearSearch = new TagBuilder("li");
                clearSearch.AddCssClass("nav-item");

                var clearSearchAnchor = new TagBuilder("a");
                clearSearchAnchor.AddCssClass("nav-link");
                clearSearchAnchor.AddCssClass("btn-secondary");
                clearSearchAnchor.AddCssClass("btn-sm");

                var currentUrl = _urlHelperFactory.GetUrlHelper(ViewContext).ActionContext.RouteData.Values
                    .FirstOrDefault(x => x.Key.Equals("page", StringComparison.CurrentCultureIgnoreCase)).Value.ToString();
                currentUrl = _urlHelperFactory.GetUrlHelper(ViewContext).Page(currentUrl, new
                {
                    pageNo = 1
                });
                clearSearchAnchor.Attributes.Add("href", currentUrl);
                clearSearchAnchor.InnerHtml.AppendHtml(_localizer[SharedResource.ClearSearch]);

                clearSearch.InnerHtml.AppendHtml(clearSearchAnchor);

                navbarRight.InnerHtml.AppendHtml(clearSearch);
            }

            #endregion ClearSearch

            #region RowCount

            var rowCount = new TagBuilder("li");
            rowCount.AddCssClass("nav-item");

            var buttonRowCount = new TagBuilder("a");
            buttonRowCount.Attributes.Add("href", "#");
            buttonRowCount.AddCssClass("item-count");
            buttonRowCount.AddCssClass("nav-link");
            buttonRowCount.AddCssClass("btn-sm");
            buttonRowCount.InnerHtml.AppendHtml(Rows.ToString()).AppendHtml(" آیتم");

            rowCount.InnerHtml.AppendHtml(buttonRowCount);
            navbarRight.InnerHtml.AppendHtml(rowCount);

            #endregion RowCount

            var collapse = new TagBuilder("div");
            collapse.AddCssClass("collapse");
            collapse.AddCssClass("navbar-collapse");
            collapse.Attributes.Add("id", "searchNavBar");
            collapse.InnerHtml.AppendHtml(navbarLeft).AppendHtml(navbarRight);

            var buttonToggler = new TagBuilder("button");
            buttonToggler.AddCssClass("navbar-toggler");
            buttonToggler.Attributes.Add("type", "button");
            buttonToggler.Attributes.Add("data-toggle", "collapse");
            buttonToggler.Attributes.Add("data-target", "#searchNavBar");
            buttonToggler.Attributes.Add("aria-controls", "searchNavBar");

            var buttonTogglerIcon = new TagBuilder("span");
            buttonTogglerIcon.AddCssClass("navbar-toggler-icon");

            var buttonTogglerTitle = new TagBuilder("span");
            buttonTogglerTitle.AddCssClass("navbar-toggler-title");
            buttonTogglerTitle.InnerHtml.AppendHtml("جستجو و مرتب سازی");

            buttonToggler.InnerHtml.AppendHtml(buttonTogglerIcon).AppendHtml(buttonTogglerTitle);

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "nav";
            output.AddClass("navbar", HtmlEncoder.Default);
            output.AddClass("navbar-expand-md", HtmlEncoder.Default);
            output.AddClass("navbar-light", HtmlEncoder.Default);
            output.AddClass("bg-light", HtmlEncoder.Default);
            output.AddClass("mb-2", HtmlEncoder.Default);
            output.AddClass("p-0", HtmlEncoder.Default);
            output.Content.AppendHtml(buttonToggler).AppendHtml(collapse);

            if (isTriggered != null && isTriggered == true)
            {
                var searchOptionsDiv = new TagBuilder("div");
                searchOptionsDiv.AddCssClass("row ");
                searchOptionsDiv.AddCssClass("justify-content-center");
                searchOptionsDiv.AddCssClass("align-items-center");
                searchOptionsDiv.AddCssClass("mb-2");
                var col = new TagBuilder("div");
                col.AddCssClass("col-12");
                var card = new TagBuilder("div");
                card.AddCssClass("card");
                var cardBody = new TagBuilder("div");
                cardBody.AddCssClass("card-body search-options");
                cardBody.AddCssClass("text-right");
                var h5 = new TagBuilder("h5");
                h5.InnerHtml.AppendHtml("جستجو بر اساس:");
                var ulBreadcrumb = new TagBuilder("ul");
                ulBreadcrumb.AddCssClass("info-breadcrumb");
                ulBreadcrumb.AddCssClass("no-border-bottom");

                foreach (var (key, value) in searchOptions)
                {
                    var li = new TagBuilder("li");
                    var b = new TagBuilder("b");
                    b.AddCssClass("text-dark");
                    b.InnerHtml.AppendHtml(key).AppendHtml(value != null ? ":" : "");
                    li.InnerHtml.AppendHtml(b).AppendHtml($" {value}");
                    ulBreadcrumb.InnerHtml.AppendHtml(li);
                }
                cardBody.InnerHtml.AppendHtml(h5).AppendHtml(ulBreadcrumb);
                card.InnerHtml.AppendHtml(cardBody);
                col.InnerHtml.AppendHtml(card);
                searchOptionsDiv.InnerHtml.AppendHtml(col);
                output.PostElement.AppendHtml(searchOptionsDiv);
            }
        }
    }
}