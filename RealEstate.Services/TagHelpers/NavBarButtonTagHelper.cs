using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RealEstate.Services.TagHelpers
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

        private class UrlChecker
        {
            public string Link { get; set; }
            public string Current { get; set; }
            public bool Matched { get; set; }
        }

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
                var link = string.IsNullOrEmpty(Action)
                    ? urlHelper.Page(Page)
                    : string.IsNullOrEmpty(Controller)
                        ? urlHelper.Action(Action)
                        : urlHelper.Action(Action, Controller);
                var current = string.Join("", _actionContextAccessor.ActionContext.RouteData.Values.Values);
                current = current.EndsWith("/Index", StringComparison.CurrentCultureIgnoreCase)
                    ? link.EndsWith("/Index", StringComparison.CurrentCultureIgnoreCase)
                        ? current
                        : current.Split("/Index")[0]
                    : current;

                var linkArr = link?.Split("/");
                var currentArr = current?.Split("/");
                linkArr = linkArr?.Skip(string.IsNullOrEmpty(linkArr[0]) ? 1 : 0).ToArray();
                currentArr = currentArr?.Skip(string.IsNullOrEmpty(currentArr[0]) ? 1 : 0).ToArray();
                if (linkArr == null || linkArr.Length == 0 || currentArr == null || currentArr.Length == 0)
                {
                    output.SuppressOutput();
                    return;
                }

                List<UrlChecker> checker;
                if (linkArr.Length > currentArr.Length)
                {
                    checker = (from url in linkArr
                               let index = Array.IndexOf(linkArr, url)
                               let crnt = index >= currentArr.Length || index >= linkArr.Length
                                   ? ""
                                   : currentArr[index]
                               let matched = url == crnt
                               select new UrlChecker
                               {
                                   Link = url,
                                   Current = crnt,
                                   Matched = matched
                               }).ToList();
                }
                else
                {
                    checker = (from url in currentArr
                               let index = Array.IndexOf(currentArr, url)
                               let lnk = index >= currentArr.Length || index >= linkArr.Length
                                   ? ""
                                   : linkArr[index]
                               let matched = url == lnk
                               select new UrlChecker
                               {
                                   Link = lnk,
                                   Current = url,
                                   Matched = matched
                               }).ToList();
                }

                var success = checker.All(x => x.Matched);
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

                output.Attributes.Add("href", link);
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