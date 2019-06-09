using RealEstate.Base;
using System;
using System.Globalization;

namespace RealEstate.Services.Extensions
{
    public static class FeatureFixExtension
    {
        public static string TranslateFeature(this string featureName, string featureValue, DateTime lastAudit)
        {
            if (string.IsNullOrEmpty(featureName) || string.IsNullOrEmpty(featureValue))
                return default;

            var date = $"{lastAudit.GregorianToPersian(true)} - {new PersianDateTime(lastAudit).ToRelativeString()}";

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

                    var finalYear = currentYear - processedYear;
                    var term = finalYear == 0 ? "نوساز" : $"{finalYear} سال ساخت";
                    return $"{term} ( {processedYear} )";

                case "متراژ":
                    var isMeter = int.TryParse(featureValue, out var priceByMeter);
                    var term2 = isMeter
                        ? $"{priceByMeter} متری"
                        : $"متراژ: {featureValue}";
                    return term2;

                case "بر زمین":
                    return $"{featureName} : {featureValue} متر";

                case "وام":
                case "قیمت نهایی":
                    featureValue = featureValue.Length <= 3 ? $"{featureValue}/000/000" : featureValue;
                    var words1 = featureValue.CurrencyToWords();
                    return $"{featureName} : {words1} تومان ( تا تاریخ {date} )";

                case "قیمت هر متر":
                    var words2 = featureValue.CurrencyToWords();

                    return $"متری {words2} تومان ( تا تاریخ {date} )";

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