using Microsoft.EntityFrameworkCore.Internal;
using RealEstate.Base;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class SearchExtension
    {
        public static bool IsUnderCondition<T>(this T model) where T : BaseSearchModel
        {
            const string jsonTerm = "Json";
            var type = model.GetType();
            var properties = type.GetPublicProperties().Where(x => x.GetSearchParameter() != null && !x.Name.Equals(nameof(model.PageNo))).ToList();
            if (properties?.Any() != true)
                return true;

            var conditions = new List<string>();
            foreach (var property in properties)
            {
                var key = property.Name;
                var value = property.GetValue(model);
                var searchProperty = property.GetSearchParameter();

                var hasJson = searchProperty.Type != null && key.EndsWith(jsonTerm);
                if (hasJson)
                {
                    if (!(value is string jsonString))
                        continue;

                    if (!jsonString.Equals("") || !jsonString.Equals("[]"))
                        conditions.Add(property.Name);
                }
                else
                {
                    switch (value)
                    {
                        case string propValueString:
                            if (!string.IsNullOrEmpty(propValueString))
                                conditions.Add(property.Name);
                            break;

                        case bool propValueBool:
                            if (propValueBool)
                                conditions.Add(property.Name);
                            break;

                        case decimal propValueNum:
                            if (propValueNum > 0)
                                conditions.Add(property.Name);
                            break;

                        case double propValueNum:
                            if (propValueNum > 0)
                                conditions.Add(property.Name);
                            break;

                        case int propValueNum:
                            if (propValueNum > 0)
                                conditions.Add(property.Name);
                            break;
                    }
                }
            }

            return conditions.Count > 0;
        }
    }
}