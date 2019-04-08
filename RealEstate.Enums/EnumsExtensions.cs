using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RealEstate.Base
{
    public static class EnumsExtensions
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

        public static IEnumerable<EnumResponse> FindRelatedItems<TEnumSource, TEnumDestination>(this TEnumSource @enum) where TEnumSource : Enum where TEnumDestination : Enum
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
                    Display = parsedEnum.GetDisplayName(),
                    Description = parsedEnum.GetDescription()
                };
            }
        }

        public static TEnum To<TEnum>(this int value)
        {
            return (TEnum)(object)value;
        }

        public static TEnum To<TEnum>(this string value) where TEnum : Enum
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

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