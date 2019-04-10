using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RealEstate.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Web.Components
{
    public class PaginationViewComponent : ViewComponent
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PaginationViewComponent(IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory)
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        public IViewComponentResult Invoke(ModelExpression model)
        {
            var urlHelper =
              _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var pagination = model.Model;
            if (pagination == null)
                return View(null);

            var type = pagination.GetType();
            var genericTypes = type.GenericTypeArguments;
            if (genericTypes.Length == 0)
                throw new Exception("Can't find any generic class");

            var isBaseTypeOk = genericTypes[0].HasBaseType(typeof(BaseAbstractModel));
            if (!isBaseTypeOk)
                throw new Exception("Model type should be member of BaseAbstractModel");

            var pages = (int)type.GetProperty("Pages").GetValue(pagination);
            var currentPage = (int)type.GetProperty("CurrentPage").GetValue(pagination);

            var currentUrlData = _actionContextAccessor.ActionContext.RouteData;
            if (currentUrlData?.Values == null
                || currentUrlData.Values.Count == 0
                || currentUrlData.Values.Values.Count == 0)
            {
                return View(null);
            }

            var currentUrlIsPage = currentUrlData.Values.Keys.FirstOrDefault() == "page";
            var currentUrlRoutes = currentUrlData.Values.Values;

            var final = new List<PaginationPassModel>();
            for (var x = 0; x < pages; x++)
            {
                var page = x + 1;

                string currentUrl;
                var routes = new { pageNo = page };
                if (currentUrlIsPage)
                {
                    var str = currentUrlRoutes.FirstOrDefault()?.ToString();
                    currentUrl = urlHelper.Page(str, routes);
                }
                else
                {
                    var arr = currentUrlRoutes.Take(2).Cast<string>().ToArray();
                    currentUrl = urlHelper.Action(arr[1], arr[0], routes);
                }

                final.Add(new PaginationPassModel
                {
                    Num = page,
                    IsCurrent = page == currentPage,
                    Link = currentUrl
                });
            }

            return View(final);
        }
    }
}