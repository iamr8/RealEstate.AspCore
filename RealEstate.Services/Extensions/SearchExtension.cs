using RealEstate.Base;
using System;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class SearchExtension
    {
        public static bool IsUnderCondition<T>(this T model) where T : BaseSearchModel
        {
            const string jsonTerm = "Json";
            var type = model.GetType();
            var properties = type.GetPublicProperties();
            if (properties?.Any() != true)
                return true;

            var indicator = 0;
            foreach (var property in properties)
            {
                if (property.Name.Equals(nameof(model.PageNo)))
                    continue;

                var key = property.Name;
                var value = property.GetValue(model);

                var searchProperty = property.GetSearchParameter();
                if (searchProperty == null)
                    continue;

                var hasJson = searchProperty.Type != null && key.EndsWith(jsonTerm);
                if (hasJson)
                {
                    var jsonObjectName = key.Reverse().ToString().Substring(jsonTerm.Length).Reverse().ToString();
                    var jsonObjectProperty = type.GetProperty(jsonObjectName);
                    if (jsonObjectProperty == null)
                        continue;
                }
                else
                {
                }
                switch (value)
                {
                    case string propValueString:
                    {
                        if (!string.IsNullOrEmpty(propValueString))
                            indicator++;
                        break;
                    }
                    case bool propValueBool:
                    {
                        if (propValueBool)
                            indicator++;
                        break;
                    }
                }
            }

            return indicator > 0;
        }
    }
}