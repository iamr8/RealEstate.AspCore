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
                var properties = type.GetPublicProperties().Where(x => x.GetSearchParameter() != null && !x.Name.Equals(nameof(PageNo)) && !x.Name.EndsWith("Id", StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (properties?.Any() != true)
                    return true;

                var conditions = new List<string>();
                foreach (var property in properties)
                {
                    var key = property.Name;
                    var value = property.GetValue(this);
                    var searchProperty = property.GetSearchParameter();

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
    }
}