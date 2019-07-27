using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RealEstate.Base;
using RealEstate.Services.BaseLog;
using RestSharp.Extensions;

namespace RealEstate.Services.ViewComponents
{
    public class PaginationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ModelExpression model)
        {
            var testPage = new PaginationViewModel<string>();
            var pagination = model.Model;
            if (pagination == null)
                return View(null);

            var type = pagination.GetType();
            var genericTypes = type.GenericTypeArguments;
            if (genericTypes.Length == 0)
                throw new Exception("Can't find any generic class");

            var isBaseTypeOk = genericTypes[0].IsSubclassOf(typeof(BaseLogViewModel));
            if (!isBaseTypeOk)
                throw new Exception("Model type should be member of BaseLogViewModel");

            var pages = (int)type.GetProperty(nameof(testPage.Pages)).GetValue(pagination);
            var currentPage = (int)type.GetProperty(nameof(testPage.CurrentPage)).GetValue(pagination);

            var currentUrlData = ViewContext.RouteData;
            if (currentUrlData?.Values == null
                || currentUrlData.Values.Count == 0
                || currentUrlData.Values.Values.Count == 0)
            {
                return View(null);
            }

            var currentUrlIsPage = currentUrlData.Values.Keys.FirstOrDefault() == "page";
            var currentUrlRoutes = currentUrlData.Values.Values;

            var final = new List<PaginationPassModel>();
            var routeTemplate = new Dictionary<string, string>();
            try
            {
                var queryString = ViewContext.HttpContext.Request.QueryString.Value;
                if (!string.IsNullOrEmpty(queryString))
                {
                    queryString = queryString.Substring(queryString.StartsWith("?") ? 1 : 0);
                    if (!string.IsNullOrEmpty(queryString))
                    {
                        routeTemplate = queryString.Split("&").Where(x => x.Split("=")[0] != "pageNo")
                            .ToDictionary(x => x.Split("=")[0], x => x.Split("=")[1].UrlDecode());
                    }
                }
            }
            catch
            {
            }
            for (var x = 0; x < pages; x++)
            {
                var page = x + 1;
                var routes = new Dictionary<string, string>(routeTemplate)
                {
                    {
                        "pageNo", page.ToString()
                    }
                };

                string currentUrl;
                if (currentUrlIsPage)
                {
                    var str = currentUrlRoutes.FirstOrDefault()?.ToString();
                    currentUrl = Url.Page(str, routes);
                }
                else
                {
                    var arr = currentUrlRoutes.Take(2).Cast<string>().ToArray();
                    currentUrl = Url.Action(arr[1], arr[0], routes);
                }

                final.Add(new PaginationPassModel
                {
                    Num = page,
                    IsCurrent = page == currentPage,
                    Link = currentUrl,
                });
            }

            return View(final);
        }
    }
}