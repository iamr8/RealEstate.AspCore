using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RealEstate.Base
{
    public static class JsonExtensions
    {
        public static string JsonConversion<TEntity, TModel>(this ICollection<TEntity> model, Func<TEntity, TModel> mapTo, bool includeDeletedItems = true)
        {
            if (model?.Any() != true)
                return "[]";

            var json = includeDeletedItems
                ? model.Count > 0
                    ? JsonConvert.SerializeObject(model.ToList().Select(mapTo.Invoke), JsonNetSetting)
                    : "[]"
                : model.ToList().Count > 0
                    ? JsonConvert.SerializeObject(model.ToList().Select(mapTo.Invoke), JsonNetSetting)
                    : "[]";

            return json;
        }

        public static string JsonSetAccessor(this string json)
        {
            if (string.IsNullOrEmpty(json) || json == "null")
                return "[]";

            return json;
        }

        private const string JsonItemDivider = "....";
        private const string JsonPropertyDivider = "|";
        private const string JsonPropertyMemberDivider = "..";

        public static string EncodeJson(this string json, Type modelType)
        {
            var listType = typeof(List<>).MakeGenericType(modelType);

            var jsonList = JsonConvert.DeserializeObject(json, listType);
            if (jsonList == null)
                return default;

            if (!(jsonList is IList jsons))
                return default;

            var itemsString = new StringBuilder();
            var itemIndicator = 0;
            foreach (var item in jsons)
            {
                var itemProps = item.GetPublicProperties();
                if (itemProps == null || itemProps.Count <= 0)
                    continue;

                var itemString = new StringBuilder();
                var propertyIndicator = 0;

                var oneProperty = modelType.GetProperties().Length == 1;
                foreach (var prop in itemProps)
                {
                    var propValue = prop.GetValue(item);
                    var key = prop.GetJsonProperty();

                    if (oneProperty)
                    {
                        itemString.Append(propValue);
                    }
                    else
                    {
                        itemString
                            .Append(key)
                            .Append(JsonPropertyMemberDivider)
                            .Append(propValue);
                    }

                    if (propertyIndicator != itemProps.Count - 1)
                        itemString.Append(JsonPropertyDivider);

                    propertyIndicator++;
                }

                itemsString.Append(itemString);
                if (itemIndicator != jsons.Count - 1)
                    itemsString.Append(JsonItemDivider);

                itemIndicator++;
            }

            var result = itemsString.ToString();
            return result;
        }

        public static List<TModel> AddNoneToLast<TModel>(this List<TModel> list)
        {
            var result = list;
            var none = Activator.CreateInstance<TModel>();
            if (result?.Any() == true)
            {
                result.Add(none);
            }
            else
            {
                result = new List<TModel>
                {
                    none
                };
            }

            return result;
        }

        public static List<TModel> JsonGetAccessor<TModel>(this string json)
        {
            return string.IsNullOrEmpty(json)
                ? default
                : JsonConvert.DeserializeObject<List<TModel>>(json);
        }

        public static string DecodeJson<TModel>(this string json)
        {
            // f..ert|t..ert|nm..متراژ|id..15bf9d15-07bc-4f3c-8339-8192c8fd0c18
            var modelType = typeof(TModel);
            if (string.IsNullOrEmpty(json))
                return default;

            if (json == "[]")
                return default;

            if (json.StartsWith("["))
                return json;

            var splitByItem = json.Split(JsonItemDivider);
            if (splitByItem?.Any() != true)
                return json;

            var properties = typeof(TModel).GetProperties();
            var oneProperty = properties.Length == 1;
            var firstProperty = properties.FirstOrDefault();

            var items = (from item in splitByItem
                         select item.Split(JsonPropertyDivider) into splitByProperty
                         where splitByProperty?.Any() == true
                         select (from property in splitByProperty
                                 select property.Split(JsonPropertyMemberDivider)
                                 into split
                                 where split?.Any() == true
                                 let key = oneProperty ? firstProperty.GetJsonProperty() : split[0]
                                 let propertyInfo = key.GetProperty<TModel>()
                                 let type = propertyInfo.PropertyType
                                 let isNumeric = type == typeof(int) || type == typeof(double) || type == typeof(decimal) || type == typeof(long)
                                 let value = oneProperty
                                     ? isNumeric ? split[0] : $"\"{split[0]}\""
                                     : isNumeric ? split[1] : $"\"{split[1]}\""
                                 select $"\"{key}\":{value}").ToList()
                         into props
                         select string.Join(", ", props)
                         into propsString
                         select "{" + propsString + "}").ToList();
            var itemsString = string.Join(", ", items);
            var array = $"[{itemsString}]";
            return array;
        }

        public static string JsonSetAccessor<TModel>(this List<TModel> obj) where TModel : class
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string JsonConversion<TModel, TOutput>(this List<TModel> model, Func<TModel, TOutput> selector) where TModel : class
        {
            if (model?.Any() != true)
                return "[]";

            var result = new List<TOutput>();
            foreach (var source in model)
                result.Add(selector.Invoke(source));

            var json = result.Count > 0
                ? JsonConvert.SerializeObject(result, JsonNetSetting)
                : "[]";

            return json;
        }

        public static string JsonConversion<TModel>(this List<TModel> model) where TModel : class
        {
            if (model?.Any() != true)
                return "[]";

            var json = model.Count > 0
                ? JsonConvert.SerializeObject(model, JsonNetSetting)
                : "[]";

            return json;
        }

        public static TEntity JsonConversion<TEntity>(this string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            var obj = JsonConvert.DeserializeObject<TEntity>(json, JsonNetSetting);
            return obj;
        }

        public class NullToEmptyContractResolver : DefaultContractResolver
        {
            protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
            {
                var provider = base.CreateMemberValueProvider(member);
                if (member.MemberType != MemberTypes.Property) return provider;

                var propType = ((PropertyInfo)member).PropertyType;
                if (propType.IsGenericType
                    && propType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return new EmptyListValueProvider(provider, propType);
                }

                return propType == typeof(string)
                    ? new NullToEmptyStringValueProvider(provider)
                    : provider;
            }

            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToPascalCase(true, new CultureInfo("fa-IR")).MakeInitialLowerCase();
            }

            private class EmptyListValueProvider : IValueProvider
            {
                private readonly IValueProvider _provider;
                private readonly object _defaultValue;

                public EmptyListValueProvider(IValueProvider innerProvider, Type listType)
                {
                    _provider = innerProvider;
                    _defaultValue = Activator.CreateInstance(listType);
                }

                public void SetValue(object target, object value)
                {
                    _provider.SetValue(target, value ?? _defaultValue);
                }

                public object GetValue(object target)
                {
                    return _provider.GetValue(target) ?? _defaultValue;
                }
            }

            private sealed class NullToEmptyStringValueProvider : IValueProvider
            {
                private readonly IValueProvider _provider;

                public NullToEmptyStringValueProvider(IValueProvider provider)
                {
                    _provider = provider;
                }

                public object GetValue(object target)
                {
                    return _provider.GetValue(target) ?? "";
                }

                public void SetValue(object target, object value)
                {
                    _provider.SetValue(target, value);
                }
            }
        }

        private static bool IsUpperCase(this string inputString)
        {
            return Regex.IsMatch(inputString, "^[A-Z]+$");
        }

        private static string MakeInitialLowerCase(this string word)
        {
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        private static string ToPascalCase(this string text, bool removeUnderscores, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            text = text.Replace("_", " ");
            var separator = removeUnderscores ? string.Empty : "_";
            var strArray = text.Split(' ');
            if (strArray.Length <= 1 && !IsUpperCase(strArray[0]))
                return strArray[0].Substring(0, 1).ToUpper(culture) + strArray[0].Substring(1);
            for (var index = 0; index < strArray.Length; ++index)
            {
                if (strArray[index].Length <= 0) continue;

                var str = strArray[index];
                var inputString = str.Substring(1);
                if (IsUpperCase(inputString))
                    inputString = inputString.ToLower(culture);
                var upper = char.ToUpper(str[0], culture);
                strArray[index] = (int)upper + inputString;
            }
            return string.Join(separator, strArray);
        }

        public static JsonSerializerSettings JsonNetSetting => new JsonSerializerSettings
        {
            Error = (serializer, err) => err.ErrorContext.Handled = true,
            DefaultValueHandling = DefaultValueHandling.Populate,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ContractResolver = new NullToEmptyContractResolver(),
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };
    }
}