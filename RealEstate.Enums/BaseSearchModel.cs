using Microsoft.AspNetCore.Mvc;
using RealEstate.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Base
{
    public abstract class BaseSearchModel : AdminSearchConditionViewModel
    {
        [SearchParameter("pageNo")]
        [HiddenInput]
        public int PageNo { get; set; }

        public bool IsUnderCondition
        {
            get
            {
                const string jsonTerm = "Json";
                var type = GetType();
                var properties = type.GetPublicProperties().Where(x => x.GetSearchParameterAttribute() != null && !x.Name.Equals(nameof(PageNo)) && !x.Name.EndsWith("Id", StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (properties?.Any() != true)
                    return true;

                var conditions = new List<string>();
                foreach (var property in properties)
                {
                    var key = property.Name;
                    var value = property.GetValue(this);
                    var searchProperty = property.GetSearchParameterAttribute();

                    if (value == null)
                        continue;

                    switch (value)
                    {
                        case Enum propValueEnum:
                            conditions.Add(property.Name);
                            break;

                        case string propValueString:
                            if (!string.IsNullOrEmpty(propValueString))
                                if (searchProperty.Type != null && key.EndsWith(jsonTerm))
                                {
                                    if (!propValueString.Equals("") || !propValueString.Equals("[]"))
                                        conditions.Add(property.Name);
                                }
                                else
                                {
                                    conditions.Add(property.Name);
                                }
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

                return conditions.Count > 0;
            }
        }

        public Dictionary<string, object> GetSearchParameters()
        {
            var routeValues = new Dictionary<string, object>();

            var properties = GetType().GetPublicProperties().Where(x => x.GetValue(this) != null).ToList();
            if (properties?.Any() != true)
                return default;

            foreach (var property in properties)
            {
                var value = property.GetValue(this).ToString();
                var searchParameterAttribute = property.GetSearchParameterAttribute();
                if (searchParameterAttribute == null)
                    continue;

                var searchParameter = searchParameterAttribute.ParameterName;
                if (string.IsNullOrEmpty(searchParameter))
                    continue;

                if (searchParameterAttribute.Type != null)
                {
                    if (property.PropertyType != typeof(string))
                        continue;

                    var encodeJson = value.EncodeJson(searchParameterAttribute.Type);
                    routeValues.Add(searchParameter, encodeJson);
                }
                else
                {
                    routeValues.Add(searchParameter, value);
                }
            }

            return routeValues;
        }
    }
}