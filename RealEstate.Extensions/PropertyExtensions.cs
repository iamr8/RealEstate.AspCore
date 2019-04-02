using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using RealEstate.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;

namespace RealEstate.Extensions
{
    public static class PropertyExtensions
    {
        public static TAttribute GetPropertyAttribute<TModel, TAttribute>(
            this Expression<Func<TModel, object>> expression) where TAttribute : Attribute
        {
            var propertyInfo = expression.GetProperty();
            return propertyInfo?.GetCustomAttributes(false).OfType<TAttribute>().FirstOrDefault();
        }

        public static PropertyInfo GetProperty<TModel>(this Expression<Func<TModel, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            var memberExpression = lambda.Body is UnaryExpression unaryExpression
                ? (MemberExpression)unaryExpression.Operand
                : (MemberExpression)lambda.Body;

            return memberExpression.Member as PropertyInfo;
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

        public static IHtmlContent GetValidator<TModel>(this Expression<Func<TModel, object>> property)
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

        public static string GetDisplayName<TModel>(this Expression<Func<TModel, object>> property)
        {
            var resourceManager = new ResourceManager(typeof(SharedResource));

            var display = property.GetPropertyAttribute<TModel, DisplayAttribute>();
            return display != null
                ? display.ResourceType != null
                    ? resourceManager.GetString(display.Name)
                    : display.Name
                : property.Name;
        }

        public static string GetJsonProperty<TModel>(this Expression<Func<TModel, object>> property)
        {
            var json = property.GetPropertyAttribute<TModel, JsonPropertyAttribute>();
            if (json == null)
            {
                var propertyInfo = property.GetProperty();
                var contract = JsonExtensions.JsonNetSetting.ContractResolver;
                if (contract is JsonExtensions.NullToEmptyContractResolver customContract)
                {
                    var resolved = customContract.GetResolvedPropertyName(propertyInfo.Name);
                    return resolved;
                }

                return propertyInfo.Name;
            }

            return json.PropertyName;
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