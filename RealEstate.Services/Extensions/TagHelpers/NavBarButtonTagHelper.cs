using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RealEstate.Services.Extensions.TagHelpers
{
    [HtmlTargetElement("nav-button", Attributes = "icon, subject", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class NavBarButtonTagHelper : TagHelper
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public NavBarButtonTagHelper(IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        [HtmlAttributeName("subject")]
        public string Subject { get; set; }

        [HtmlAttributeName("class")]
        public string Class { get; set; }

        [HtmlAttributeName("icon")]
        public string Icon { get; set; }

        [HtmlAttributeName("page")]
        public string Page { get; set; }

        [HtmlAttributeName("action")]
        public string Action { get; set; }

        [HtmlAttributeName("controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("hideOnTrigger")]
        public bool HideOnTrigger { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper =
                _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var contentContext = await output.GetChildContentAsync().ConfigureAwait(false);
            var content = contentContext.GetContent();

            var classes = new List<string>();
            if (string.IsNullOrEmpty(Subject))
            {
                output.SuppressOutput();
                return;
            }

            if (string.IsNullOrEmpty(Page) && string.IsNullOrEmpty(Action))
            {
                output.TagName = "span";
            }
            else
            {
                var linkUrl = string.IsNullOrEmpty(Action)
               ? urlHelper.Page(Page)
               : string.IsNullOrEmpty(Controller)
                   ? urlHelper.Action(Action)
                   : urlHelper.Action(Action, Controller);

                var currentUrlData = _actionContextAccessor.ActionContext.RouteData;
                if (currentUrlData?.Values == null
                    || currentUrlData.Values.Count == 0
                    || currentUrlData.Values.Values.Count == 0)
                {
                    output.SuppressOutput();
                    return;
                }

                var currentUrlIsPage = currentUrlData.Values.Keys.FirstOrDefault() == "page";
                var currentUrlRoutes = currentUrlData.Values.Values;
                string currentUrl;
                if (currentUrlIsPage)
                {
                    currentUrl = urlHelper.Page(currentUrlRoutes.FirstOrDefault().ToString());
                }
                else
                {
                    var acCont = currentUrlRoutes.Take(2).Cast<string>().ToArray();
                    currentUrl = urlHelper.Action(acCont[1], acCont[0]);
                }

                string[] Normalize(string array, bool isCurrent)
                {
                    var isPage = isCurrent ? currentUrlIsPage : !string.IsNullOrEmpty(Page) && string.IsNullOrEmpty(Action);
                    if (isPage)
                    {
                        if (array.Equals(@"/"))
                            array += "Index";
                    }
                    else if (array.Count(x => x.Equals('/')) == 1)
                    {
                        array += "/Index";
                    }

                    return array.Trim().Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                }

                var currentUrlArray = Normalize(currentUrl, true);
                var linkUrlArray = Normalize(linkUrl, false);

                const int requiredMatches = 2;
                var biggerValue = currentUrlArray.Length > linkUrlArray.Length
                    ? currentUrlArray.Length
                    : linkUrlArray.Length;
                var minimumMatches = currentUrlArray.Length >= requiredMatches && linkUrlArray.Length >= requiredMatches
                    ? requiredMatches
                    : biggerValue;

                var matches = 0;
                for (var index = 0; index < minimumMatches; index++)
                {
                    if (index == currentUrlArray.Length || index == linkUrlArray.Length)
                        break;

                    var currentUrlItem = currentUrlArray[index];
                    var linkUrlItem = linkUrlArray[index];

                    if (currentUrlItem == linkUrlItem)
                        matches++;
                }

                var success = matches >= minimumMatches;
                if (success)
                {
                    if (HideOnTrigger)
                    {
                        output.SuppressOutput();
                        return;
                    }
                    classes.Add("active");
                }
                if (!string.IsNullOrEmpty(Class))
                    classes.AddRange(Class.Split(' '));

                output.Attributes.Add("href", linkUrl);
                output.TagName = "a";
            }

            var imgWrapper = new TagBuilder("div");
            imgWrapper.AddCssClass("img");

            var img = new TagBuilder("img");
            img.Attributes.Add("src", urlHelper.Content(Icon));
            img.Attributes.Add("alt", Subject);
            imgWrapper.InnerHtml.AppendHtml(img);

            var txtWrapper = new TagBuilder("div");
            txtWrapper.AddCssClass("text");

            txtWrapper.InnerHtml.Append(Subject);

            if (!string.IsNullOrEmpty(content))
            {
                var subItems = new TagBuilder("div");
                subItems.AddCssClass("sub-items");
                subItems.InnerHtml.AppendHtml(content);

                output.PostElement.AppendHtml(subItems);
            }

            output.AddClass("nav-link", HtmlEncoder.Default);
            if (!string.IsNullOrEmpty(content))
                output.AddClass("wrapper", HtmlEncoder.Default);
            if (classes.Count > 0)
                foreach (var @class in classes)
                    output.AddClass(@class, HtmlEncoder.Default);

            output.Content.AppendHtml(imgWrapper).AppendHtml(txtWrapper);
            output.TagMode = TagMode.StartTagAndEndTag;
            base.Process(context, output);
        }
    }
}