using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace RealEstate.Web
{
    public static class PageRouterExtension
    {
        private static string Manage => nameof(RealEstate.Web.Pages.Manage);

        private static string FixPageModelName(this MemberInfo page)
        {
            return page.Name.Replace("Model", "");
        }

        public static string Page(this Type pageType)
        {
            var nameSpace = pageType?.Namespace;
            if (string.IsNullOrEmpty(nameSpace))
                throw new Exception("Namespace must be not null or empty");

            var splitByDots = nameSpace.Split('.');
            if (!splitByDots.Any())
                throw new Exception("Splitted Namespace must be not empty");

            foreach (var text in splitByDots)
            {
                if (text != Manage)
                    continue;

                var indexInString = nameSpace.IndexOf(text, StringComparison.Ordinal);
                nameSpace = nameSpace.Substring(indexInString);
                break;
            }

            var address = "/";
            splitByDots = nameSpace.Split('.');
            if (splitByDots.Length > 0)
                address = splitByDots.Aggregate(address, (current, text) => current + $"{text}/");

            address += pageType.FixPageModelName();
            return address;
        }
    }
}