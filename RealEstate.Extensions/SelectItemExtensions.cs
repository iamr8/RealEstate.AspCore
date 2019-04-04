using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Extensions
{
    public static class SelectItemExtensions
    {
        public static List<SelectListItem> AddNone(this IEnumerable<SelectListItem> selectList)
        {
            return selectList.Prepend(new SelectListItem
            {
                Value = string.Empty,
                Text = "---"
            }).ToList();
        }

        public static List<SelectListItem> SelectFirstItem(this IEnumerable<SelectListItem> selectList)
        {
            var selectListItems = selectList.ToList();
            if (selectListItems.Count == 0)
                return default;

            for (var i = 0; i < selectListItems.Count; i++)
                if (i == 0)
                    selectListItems[i].Selected = true;

            return selectListItems;
        }
    }
}