using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Web
{
    public static class PageRouterExtension
    {
        public static string[] Namespaces(this Type pageType)
        {
            var nameSpace = pageType?.Namespace;
            if (string.IsNullOrEmpty(nameSpace))
                throw new Exception("Namespace must be not null or empty");

            var splitByDots = nameSpace.Split('.');
            return splitByDots;
        }

        public static string Page(this Type pageType)
        {
            var namespaces = pageType.Namespaces();
            var manageNamespace = new List<string>();
            var crossed = false;
            foreach (var text in namespaces)
            {
                if (text == nameof(RealEstate.Web.Pages.Manage))
                    crossed = true;

                if (crossed)
                    manageNamespace.Add(text);
            }

            var address = "/";
            if (manageNamespace.Count > 0)
                address = manageNamespace.Aggregate(address, (current, text) => current + $"{text}/");

            address += pageType.Name.Replace("Model", "");
            return address;
        }
    }
}