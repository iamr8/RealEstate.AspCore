using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using RealEstate.Base.Attributes;
using RealEstate.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;

namespace RealEstate.Base
{
    public static class PropertyExtensions
    {
        public static TAttribute GetEnumAttribute<TAttribute>(this Enum enumMember)
            where TAttribute : Attribute
        {
            var enumType = enumMember.GetType();
            var enumName = enumMember.ToName();
            var memberInfos = enumType.GetMember(enumName);
            var memberInfo = memberInfos.Single();
            return memberInfo.GetCustomAttribute<TAttribute>();
        }

        public static TAttribute GetPropertyAttribute<TModel, TAttribute>(
            this Expression<Func<TModel, object>> expression) where TAttribute : Attribute
        {
            var propertyInfo = expression.GetProperty();
            return propertyInfo?.GetCustomAttributes(false).OfType<TAttribute>().FirstOrDefault();
        }

        public static SearchParameterAttribute GetSearchParameterAttribute(this PropertyInfo property)
        {
            if (property == null)
                return default;

            var searchParameterAttribute = property.GetPropertyAttribute<SearchParameterAttribute>();
            return searchParameterAttribute;
        }

        public static string GetMemberName<T>(this Expression<T> expression)
        {
            switch (expression.Body)
            {
                case MemberExpression m:
                    return m.Member.Name;

                case UnaryExpression u when u.Operand is MemberExpression m:
                    return m.Member.Name;

                default:
                    throw new NotImplementedException(expression.GetType().ToString());
            }
        }

        public static List<Type> GetBaseTypes(this Type type)
        {
            var nestedTypes = new List<Type>();
            var currentType = type;
            var found = false;
            do
            {
                nestedTypes.Add(currentType);

                if (currentType.IsAbstract)
                    found = true;

                currentType = currentType.BaseType;
            } while (found == false);

            return nestedTypes;
        }

        public static List<PropertyInfo> SortByOrder(this Type type)
        {
            if (type == null)
                return default;

            var types = type.GetBaseTypes();
            types.Reverse();

            if (types?.Any() != true)
                return default;

            var result = new List<PropertyInfo>();
            foreach (var typee in types)
            {
                var properties = typee.GetTypeInfo().DeclaredProperties.ToList();
                if (properties?.Any() != true)
                    continue;

                var sortDic = properties.ToDictionary(x => x, x => x.GetCustomAttribute<OrderAttribute>()?.X ?? 100);
                var sortedProperties = sortDic.OrderBy(x => x.Value).Select(x => x.Key).ToList();
                result.AddRange(sortedProperties);
            }

            return result;
        }

        public static Dictionary<string, string> Sort(this Dictionary<string, string> propertiesDictionary, Type modelType)
        {
            if (!propertiesDictionary.Any())
                return propertiesDictionary;

            if (modelType == null)
                return propertiesDictionary;

            var sorted = modelType.SortByOrder();
            if (sorted?.Any() != true)
                return propertiesDictionary;

            var properties = modelType.GetPublicProperties();
            if (properties?.Any() != true)
                return propertiesDictionary;

            var result = sorted
                .Where(x => propertiesDictionary.Any(c => c.Key == x.GetDisplayName()))
                .ToDictionary(x => x.GetDisplayName(), x => propertiesDictionary.FirstOrDefault(c => c.Key == x.GetDisplayName()).Value);
            return result;
        }

        public static TAttribute GetPropertyAttribute<TAttribute>(
            this PropertyInfo property) where TAttribute : Attribute
        {
            return property?.GetCustomAttributes(false).OfType<TAttribute>().FirstOrDefault();
        }

        public static PropertyInfo GetProperty<TModel>(this string propertyName)
        {
            var modelType = typeof(TModel);

            var property = Array.Find(modelType.GetProperties(), x =>
            {
                if (x.Name == propertyName)
                    return true;

                if (x.GetJsonProperty() == propertyName)
                    return true;

                return false;
            });

            return property;
        }

        public static string GetJsonProperty(this PropertyInfo propertyInfo)
        {
            var json = propertyInfo.GetPropertyAttribute<JsonPropertyAttribute>();
            if (json != null)
                return json.PropertyName;

            var contractResolver = JsonExtensions.JsonNetSetting.ContractResolver;
            if (contractResolver is JsonExtensions.NullToEmptyContractResolver nullToEmptyContractResolver)
                return nullToEmptyContractResolver.GetResolvedPropertyName(propertyInfo.Name);

            return propertyInfo.Name;
        }

        public static object[] GetPropertyAttributes<TModel>(
            this Expression<Func<TModel, object>> expression)
        {
            var propertyInfo = expression.GetProperty();
            return propertyInfo?.GetCustomAttributes(false);
        }

        public static List<PropertyInfo> GetPublicProperties(this Type modelType)
        {
            return modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
        }

        public static List<PropertyInfo> GetPublicProperties<TModel>(this TModel model) where TModel : class
        {
            return model.GetType().GetPublicProperties();
        }

        public static PropertyInfo GetProperty<TModel>(this TModel model, string propertyName) where TModel : class
        {
            var type = model.GetType().GetProperty(propertyName);
            return type;
        }

        public static PropertyInfo GetProperty<TSource, TModel>(this Expression<Func<TSource, List<TModel>>> expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            var memberExpression = lambdaExpression.Body is UnaryExpression unaryExpression
                ? (MemberExpression)unaryExpression.Operand
                : (MemberExpression)lambdaExpression.Body;

            return memberExpression.Member as PropertyInfo;
        }

        public static PropertyInfo GetProperty<TSource, TModel>(this Expression<Func<TSource, TModel>> expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            var memberExpression = lambdaExpression.Body is UnaryExpression unaryExpression
                ? (MemberExpression)unaryExpression.Operand
                : (MemberExpression)lambdaExpression.Body;

            return memberExpression.Member as PropertyInfo;
        }

        public static PropertyInfo GetProperty<TModel>(this Expression<Func<TModel, object>> expression)
        {
            return expression.GetProperty<TModel, object>();
        }

        public static bool HasBaseType(this Type type, Type baseType)
        {
            try
            {
                if (type != baseType)
                    return type.BaseType == baseType || HasBaseType(type.BaseType, baseType);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IHtmlContent GetValidator<TModel>(this Expression<Func<TModel, object>> property) where TModel : class
        {
            var regTerm = string.Empty;
            var finalTerm = "data-val=\"true\"";
            var required = property.GetPropertyAttribute<TModel, RequiredAttribute>();
            if (required != null)
            {
                var finalRequired = string.Empty;
                if (required.ErrorMessageResourceType != null)
                {
                    var resourceManager = new ResourceManager(typeof(SharedResource));

                    var displayName = property.GetDisplayName();
                    var requiredString = resourceManager.GetString(required.ErrorMessageResourceName);
                    finalRequired += string.Format(requiredString, displayName);
                }
                else
                {
                    finalRequired += required.ErrorMessage;
                }

                finalTerm += $"data-val-required=\"{finalRequired}\"";
            }

            var (pattern, regex) = property.GetRegularExpression();
            if (pattern != null && regex != null)
                regTerm = $"{pattern} {regex}";

            return new HtmlString($"{finalTerm} {regTerm}");
        }

        public static string GetDescription(this Enum value)
        {
            return value.GetEnumAttribute<DescriptionAttribute>()?.Description ?? Enum.GetName(value.GetType(), value);
        }

        public static string GetDisplayName(this Enum value)
        {
            return value.GetEnumAttribute<DisplayAttribute>()?.GetName() ?? Enum.GetName(value.GetType(), value);
        }

        public static string GetDisplayName(this PropertyInfo property)
        {
            var display = property.GetCustomAttribute<DisplayAttribute>();
            return display != null
                ? display.GetName()
                : property.Name;
        }

        public static string GetDisplayName<TModel>(this Expression<Func<TModel, object>> property)
        {
            var display = property.GetPropertyAttribute<TModel, DisplayAttribute>();
            return display != null
                ? display.GetName()
                : property.Name;
        }

        public static string GetJsonProperty<TModel>(this Expression<Func<TModel, object>> property)
        {
            var json = property.GetPropertyAttribute<TModel, JsonPropertyAttribute>();
            if (json != null)
                return json.PropertyName;

            var propertyInfo = property.GetProperty();
            var contractResolver = JsonExtensions.JsonNetSetting.ContractResolver;
            if (contractResolver is JsonExtensions.NullToEmptyContractResolver nullToEmptyContractResolver)
                return nullToEmptyContractResolver.GetResolvedPropertyName(propertyInfo.Name);

            return propertyInfo.Name;
        }

        public static (IHtmlContent, IHtmlContent) GetRegularExpression<TModel>(this Expression<Func<TModel, object>> property, bool requireTag = true)
        {
            var validator = property.GetPropertyAttribute<TModel, ValueValidationAttribute>();
            if (validator != null)
            {
                return new ValueTuple<IHtmlContent, IHtmlContent>
                {
                    Item1 = requireTag
                        ? new HtmlString($"data-val-regex-pattern=\"{validator.RegularExpression}\"")
                        : new HtmlString(validator.RegularExpression),
                    Item2 = requireTag
                        ? new HtmlString($"data-val-regex=\"{validator.ErrorMessage}\"")
                        : new HtmlString(validator.ErrorMessage)
                };
            }

            var regular = property.GetPropertyAttribute<TModel, RegularExpressionAttribute>();
            if (regular != null)
            {
                return new ValueTuple<IHtmlContent, IHtmlContent>
                {
                    Item1 = requireTag
                        ? new HtmlString($"data-val-regex-pattern=\"{regular.Pattern}\"")
                        : new HtmlString(regular.Pattern),
                    Item2 = requireTag
                        ? new HtmlString($"data-val-regex=\"{regular.ErrorMessage}\"")
                        : new HtmlString(regular.ErrorMessage)
                };
            }

            return new ValueTuple<IHtmlContent, IHtmlContent>(null, null);
        }
    }
}