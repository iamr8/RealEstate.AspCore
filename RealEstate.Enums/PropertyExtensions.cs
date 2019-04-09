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

        public static Dictionary<string, object> GetSearchParameters<TSearch>(this TSearch model) where TSearch : BaseSearchModel
        {
            var routeValues = new Dictionary<string, object>();
            var properties = model.GetProperties();
            if (properties?.Any(x => x.Value != null) != true)
                return default;

            foreach (var (searchKey, searchValue) in properties.Where(x => x.Value != null))
            {
                var property = model.GetProperty(searchKey);
                if (property == null)
                    continue;

                var searchParameterAttribute = property.GetPropertyAttribute<SearchParameterAttribute>();
                if (searchParameterAttribute == null)
                    continue;

                var searchParameter = searchParameterAttribute.ParameterName;
                if (string.IsNullOrEmpty(searchParameter))
                    continue;

                routeValues.Add(searchParameter, searchValue);
            }

            return routeValues;
        }

        public static TAttribute GetPropertyAttribute<TAttribute>(
            this PropertyInfo property) where TAttribute : Attribute
        {
            return property?.GetCustomAttributes(false).OfType<TAttribute>().FirstOrDefault();
        }

        public static object[] GetPropertyAttributes<TModel>(
            this Expression<Func<TModel, object>> expression)
        {
            var propertyInfo = expression.GetProperty();
            return propertyInfo?.GetCustomAttributes(false);
        }

        public static ParameterInfo[] GetParameters(this MethodInfo method)
        {
            var retVal = method.GetParameters();
            return retVal;
        }

        public static MethodInfo GetMethodInfo(this Type parentType, string methodName)
        {
            var methodInfo = parentType.GetMethod(methodName);
            return methodInfo;
        }

        public static Dictionary<string, object> GetProperties<TModel>(this TModel model) where TModel : class
        {
            return model.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(model, null));
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
            var validator = property.GetPropertyAttribute<TModel, R8ValidatorAttribute>();
            if (validator != null)
            {
                return new ValueTuple<IHtmlContent, IHtmlContent>
                {
                    Item1 = requireTag
                        ? new HtmlString($"data-val-regex-pattern=\"{validator.Pattern}\"")
                        : new HtmlString(validator.Pattern),
                    Item2 = requireTag
                        ? new HtmlString($"data-val-regex=\"{validator.Caution}\"")
                        : new HtmlString(validator.Caution)
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