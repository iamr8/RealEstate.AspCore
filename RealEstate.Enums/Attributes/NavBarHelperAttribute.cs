using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Base.Attributes
{
    public class NavBarHelperAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public string[] Page { get; set; }

        public NavBarHelperAttribute(params string[] page)
        {
            Page = page;
        }

        public NavBarHelperAttribute(Type pageType)
        {
            Page = PageOfType(pageType).ToArray();
        }

        private static List<string> PageOfType(Type pageType)
        {
            var nameSpace = pageType?.Namespace;
            if (string.IsNullOrEmpty(nameSpace))
                throw new Exception("Namespace must be not null or empty");

            var namespaces = nameSpace.Split('.');
            var manageNamespace = new List<string>();
            var crossed = false;
            foreach (var text in namespaces)
            {
                if (text == "Manage")
                    crossed = true;

                if (crossed)
                    manageNamespace.Add(text);
            }

            manageNamespace.Add(pageType.Name.Replace("Model", ""));
            return manageNamespace;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            const string navKey = "NavHelper";
            var (key, value) = context.HttpContext.Items.FirstOrDefault(x => x.Key.Equals(navKey));
            if (value != null)
                return Task.FromResult(0);

            if (Page?.Any() != true)
                return Task.FromResult(0);

            context.HttpContext.Items[navKey] = string.Join("/", Page.Where(x => !string.IsNullOrEmpty(x)));
            return Task.FromResult(0);
        }
    }
}