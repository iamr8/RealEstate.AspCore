using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RealEstate.Base.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealEstate.Base
{
    public abstract class BaseSearchModel : AdminSearchConditionViewModel
    {
        [SearchParameter("pageNo")]
        [HiddenInput]
        public int PageNo { get; set; }

        public Dictionary<string, string> SearchOptions
        {
            get
            {
                const string jsonTerm = "Json";
                var type = GetType();
                var properties = type.GetPublicProperties().Where(x =>
                        x.GetSearchParameterAttribute() != null
                        && !x.Name.Equals(nameof(PageNo))
                        && !x.Name.EndsWith("Id", StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
                if (properties?.Any() != true)
                    return default;

                var conditions = new Dictionary<string, string>();
                foreach (var property in properties)
                {
                    string key;
                    try
                    {
                        key = property.Name.EndsWith(jsonTerm, StringComparison.CurrentCulture)
                           ? type.GetPublicProperties().FirstOrDefault(x =>
                               x.Name.Equals(property.Name.Replace(jsonTerm, "", StringComparison.CurrentCulture), StringComparison.CurrentCulture))?.GetDisplayName()
                           : property.GetDisplayName();
                    }
                    catch
                    {
                        key = property.GetDisplayName();
                    }

                    var value = property.GetValue(this);
                    var searchProperty = property.GetSearchParameterAttribute();

                    if (value == null)
                        continue;

                    switch (value)
                    {
                        case Enum propValueEnum:
                            conditions.Add(key, propValueEnum.GetDisplayName());
                            break;

                        case string propValueString:
                            if (!string.IsNullOrEmpty(propValueString))
                                if (searchProperty.Type != null && property.Name.EndsWith(jsonTerm))
                                {
                                    if (!propValueString.Equals("") || !propValueString.Equals("[]"))
                                    {
                                        var baseType = type.GetPublicProperties().FirstOrDefault(x =>
                                            x.Name.Equals(property.Name.Replace(jsonTerm, "", StringComparison.CurrentCulture), StringComparison.CurrentCulture));
                                        if (baseType == null)
                                            continue;

                                        var list = JsonConvert.DeserializeObject(propValueString, baseType.PropertyType);
                                        if (list == null)
                                            continue;

                                        if (!(list is IList listt))
                                            continue;

                                        var listResult = (from object listItem in listt
                                                          let itemProperties = listItem.GetType()
                                                              .GetPublicProperties()
                                                              .Where(x => !x.Name.EndsWith("Id", StringComparison.CurrentCulture))
                                                              .ToList()
                                                          where itemProperties?.Any() == true
                                                          select itemProperties.Select(x => new
                                                          {
                                                              Name = x.GetDisplayName(),
                                                              Value = x.GetValue(listItem).ToString()
                                                          }).ToDictionary(x => x.Name, x => x.Value)
                                                              .Sort(listItem.GetType())).ToList();
                                        var featuresList = new List<string>();
                                        foreach (var lr in listResult)
                                        {
                                            var strList = new List<string>();
                                            foreach (var (lrKey, lrValue) in lr)
                                            {
                                                if (string.IsNullOrEmpty(lrValue))
                                                    continue;

                                                var str = new StringBuilder();

                                                if (!lrKey.Equals("نام"))
                                                    str.Append(lrKey);

                                                var valueMoney = lrValue.EndsWith("000000") && double.TryParse(lrValue, out var money);

                                                str.Append(" ")
                                                    .Append(valueMoney ? lrValue.CurrencyToWords() + " تومان" : lrValue);

                                                strList.Add(str.ToString());
                                            }
                                            featuresList.Add(string.Join(" ", strList));
                                        }
                                        conditions.Add(key, string.Join(" | ", featuresList));
                                    }
                                }
                                else
                                {
                                    conditions.Add(key, propValueString);
                                }
                            break;

                        case bool propValueBool:
                            if (propValueBool)
                                conditions.Add(key, null);
                            break;

                        case decimal propValueNum:
                            if (propValueNum > 0)
                                conditions.Add(key, propValueNum.ToString());
                            break;

                        case double propValueNum:
                            if (propValueNum > 0)
                                conditions.Add(key, propValueNum.ToString());
                            break;

                        case int propValueNum:
                            if (propValueNum > 0)
                                conditions.Add(key, propValueNum.ToString());
                            break;
                    }
                }

                return conditions;
            }
        }

        public bool IsTriggered => SearchOptions.Count > 0;

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