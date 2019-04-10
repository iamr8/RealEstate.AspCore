using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstate.Base;

namespace RealEstate.Services.Extensions
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

        public static List<SelectListItem> ToSelectList<TEnum>(TEnum defaultValue, Func<TEnum, bool> predicate) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Where(predicate)
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = e.ToString(),
                    Selected = Equals(e, defaultValue)
                }).ToList();
        }

        public static List<SelectListItem> ToSelectList<TEnum>(Func<TEnum, bool> predicate) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Where(predicate)
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = e.ToString(),
                }).ToList();
        }

        public static List<SelectListItem> ToSelectList<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = e.ToString(),
                }).ToList();
        }

        public static List<SelectListItem> ToSelectList<TEnum>(TEnum defaultValue) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = e.ToString(),
                    Selected = Equals(e, defaultValue)
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