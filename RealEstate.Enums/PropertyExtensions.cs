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

        private class JsonKeyValue
        {
            public void Deconstruct(out string key, out object value)
            {
                key = Key;
                value = Value;
            }

            public string Key { get; set; }
            public object Value { get; set; }
        }

        public static Expression<Func<T, bool>> BuildPredicate<T>(this IQueryable<T> source, string propertyName, string value)
        {
            var type = source.ElementType;
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "x");
            var propExpression = Expression.Property(parameter, property);
            var valueExpression = Expression.Constant(value);
            var equal = Expression.Equal(propExpression, valueExpression);
            var expression = Expression.Lambda<Func<T, bool>>(equal, parameter);
            return expression;
        }

        public static Expression<Func<T, TResult>> BuildPredicate<T, TResult>(this IQueryable<T> source, string propertyName)
        {
            var type = source.ElementType;
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "x");
            var propExpression = Expression.Property(parameter, property);
            var expression = Expression.Lambda<Func<T, TResult>>(propExpression, parameter);
            return expression;
        }

        public static SearchParameterAttribute GetSearchParameter(this PropertyInfo property)
        {
            if (property == null)
                return default;

            var searchParameterAttribute = property.GetPropertyAttribute<SearchParameterAttribute>();
            return searchParameterAttribute;
        }

        public static Dictionary<string, object> GetSearchParameters<TSearch>(this TSearch model) where TSearch : BaseSearchModel
        {
            var routeValues = new Dictionary<string, object>();
            if (model == null)
                return default;

            var properties = model.GetPublicProperties().Where(x => x.GetValue(model) != null).ToList();
            if (properties?.Any() != true)
                return default;

            foreach (var property in properties)
            {
                var value = property.GetValue(model).ToString();
                var searchParameterAttribute = property.GetSearchParameter();
                if (searchParameterAttribute == null)
                    continue;

                var searchParameter = searchParameterAttribute.ParameterName;
                if (string.IsNullOrEmpty(searchParameter))
                    continue;

                if (searchParameterAttribute.Type != null)
                {
                    // JSON

                    if (property.PropertyType != typeof(string))
                        continue;

                    var encodeJson = value.EncodeJson(searchParameterAttribute.Type);
                    routeValues.Add(searchParameter, encodeJson);
                }
                else
                {
                    // PLAIN

                    routeValues.Add(searchParameter, value);
                }
            }

            return routeValues;
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