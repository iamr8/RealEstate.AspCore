using RealEstate.Base;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RealEstate.Services.Extensions
{
    public static class FeatureFixExtension
    {
        public static Regex FeatureRegex(this string featureName)
        {
            if (string.IsNullOrEmpty(featureName))
                return default;

            string pattern;
            switch (featureName)
            {
                case "سال ساخت":
                    pattern = "1(3|4)[0-9]{2}";
                    break;

                case "تعداد واحدهای مجتمع":
                case "متراژ":
                case "بر زمین":
                case "تعداد خواب":
                    pattern = RegexPatterns.NumbersOnly.GetDisplayName();
                    break;

                case "وام":
                case "قیمت نهایی":
                case "قیمت هر متر":
                    pattern = "\\d*000000";
                    break;

                case "مستاجر":
                    pattern = "(09|۰۹)([0-9]|[۰-۹]){9} ([\u0600-\u06FF ])+";
                    break;

                default:
                    pattern = RegexPatterns.SafeText.GetDisplayName();
                    break;
            }

            return new Regex(pattern, RegexOptions.Singleline);
        }

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

                    var error2 = year < 100 ? " <b style=\"color: red\">( خطا در ثبت )</b>" : "";
                    var finalYear = currentYear - processedYear;
                    var term = finalYear == 0 ? "نوساز" : $"{finalYear} سال ساخت";
                    return $"{term} ( {processedYear} ) {error2}";

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
                    featureValue = featureValue.Length <= 3 ? $"{featureValue}000000" : featureValue;
                    var words1 = featureValue.CurrencyToWords();
                    var error1 = featureValue.Length <= 3 ? $" <b style=\"color: red\">( {featureValue.FixCurrency()} )</b>" : "";
                    return $"{featureName} : {words1} تومان ( تا تاریخ {date} ){error1}";

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