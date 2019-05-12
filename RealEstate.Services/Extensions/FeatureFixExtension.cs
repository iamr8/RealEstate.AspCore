using System;
using System.Globalization;

namespace RealEstate.Services.Extensions
{
    public static class FeatureFixExtension
    {
        public static string TranslateFeature(this string featureName, string featureValue)
        {
            if (string.IsNullOrEmpty(featureName) || string.IsNullOrEmpty(featureValue))
                return default;

            switch (featureName)
            {
                case "سال ساخت":
                    var currentYear = new PersianCalendar().GetYear(DateTime.Now);
                    int.TryParse(featureValue, out var year);
                    var processedYear = year < 100
                        ? year <= 30
                            ? currentYear - year
                            : int.Parse($"13{year}")
                        : year;
                    return $"{currentYear - processedYear} سال ساخت ( {processedYear} )";

                case "متراژ":
                    return $"{featureValue} متری";

                case "بر زمین":
                    return $"{featureName} : {featureValue} متر";

                case "وام":
                case "قیمت نهایی":
                case "قیمت هر متر":
                    var words = featureValue.CurrencyToWords();
                    return $"{featureName} : {words} تومان";

                case "تعداد واحدهای مجتمع":
                    return $"{featureValue} واحده";

                case "تعداد خواب":
                    return $"{(featureValue == "1" ? "تک" : featureValue)} خوابه";

                default:
                    return $"{featureName} : {featureValue}";
            }
        }
    }
}