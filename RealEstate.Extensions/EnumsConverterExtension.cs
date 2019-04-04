using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace RealEstate.Extensions
{
    public static class EnumsConverterExtension
    {
        public static string ToName(this Enum value)
        {
            var enumType = value.GetType();
            return Enum.GetName(enumType, value);
        }

        public class EnumResponse
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public string Display { get; set; }
            public string Description { get; set; }
        }

        public static IEnumerable<EnumResponse> FindRelatedItems<TEnumSource, TEnumDestination>(TEnumSource @enum) where TEnumSource : Enum where TEnumDestination : Enum
        {
            var destination = typeof(TEnumDestination);

            foreach (var dest in Enum.GetValues(destination))
            {
                if (!dest.ToString().StartsWith(@enum.ToString())) continue;

                var parsedEnum = Enum.Parse(typeof(TEnumDestination), dest.ToString()) as Enum;
                yield return new EnumResponse
                {
                    Name = dest.ToString(),
                    Value = Convert.ToInt32(parsedEnum),
                    Display = parsedEnum.Display(),
                    Description = parsedEnum.Description()
                };
            }
        }

        private static TAttribute GetEnumMemberAttribute<TAttribute>(this Type enumType, string enumMemberName)
            where TAttribute : Attribute =>
            enumType.GetMember(enumMemberName).Single().GetCustomAttribute<TAttribute>();

        private static string Display(Type enumType, string name)
        {
            var result = name;

            var attribute = enumType
                .GetField(name)
                .GetCustomAttributes(inherit: false)
                .OfType<DisplayAttribute>()
                .FirstOrDefault();

            if (attribute != null)
                result = attribute.GetName();

            return result;
        }

        public static string Description(this Enum value)
        {
            var enumType = value.GetType();
            var enumMemberName = Enum.GetName(enumType, value);
            return enumType
                       .GetEnumMemberAttribute<DescriptionAttribute>(enumMemberName)
                       ?.Description
                   ?? enumMemberName;
        }

        public static string Display(this Enum value)
        {
            var enumType = value.GetType();
            var enumMemberName = Enum.GetName(enumType, value);
            return enumType
                       .GetEnumMemberAttribute<DisplayAttribute>(enumMemberName)
                       ?.GetName()
                   ?? enumMemberName;
        }

        public static TEnum To<TEnum>(this int value)
        {
            return (TEnum)(object)value;
        }

        public static TEnum To<TEnum>(this string value) where TEnum : Enum
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        public static List<int> ToList<TEnum>()
        {
            return Enum.GetValues(typeof(TEnum)).Cast<int>().ToList();
        }

        public static string[] ToArray<TEnum>()
        {
            if (!typeof(TEnum).IsEnum)
                throw new InvalidOperationException();

            return (string[])Enum.GetValues(typeof(TEnum));
        }
    }
}