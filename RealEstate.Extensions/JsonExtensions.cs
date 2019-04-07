using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RealEstate.Domain.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RealEstate.Extensions
{
    public static class JsonExtensions
    {
        public static string JsonConversion<TEntity, TModel>(this ICollection<TEntity> model, Func<TEntity, TModel> mapTo, bool includeDeletedItems = true)
            where TEntity : BaseEntity
        {
            if (model?.Any() != true)
                return "[]";

            var json = includeDeletedItems
                ? model.Count > 0
                    ? JsonConvert.SerializeObject(model.ToList().Select(mapTo.Invoke))
                    : "[]"
                : model.ToList().Count > 0
                    ? JsonConvert.SerializeObject(model.ToList().Select(mapTo.Invoke))
                    : "[]";

            return json;
        }

        public static string JsonConversion<TModel>(this List<TModel> model) where TModel : class
        {
            if (model?.Any() != true)
                return "[]";

            var json = model.Count > 0
                ? JsonConvert.SerializeObject(model)
                : "[]";

            return json;
        }

        public static TEntity JsonConversion<TEntity>(this string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            var obj = JsonConvert.DeserializeObject<TEntity>(json);
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
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ContractResolver = new NullToEmptyContractResolver(),
            Formatting = Formatting.Indented
        };
    }
}