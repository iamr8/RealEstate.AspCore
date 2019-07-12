using RealEstate.Base;
using RealEstate.Services.ServiceLayer;
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

        public static string CleanNumberDividers(this string num)
        {
            var featureValue = num
                .Replace(",", "")
                .Replace("/", "")
                .Replace(".", "")
                .Replace("،", "");
            return featureValue;
        }

        public class NormalizeFeatureStatus
        {
            public NormalizeFeatureStatus()
            {
            }

            public void Deconstruct(out string value, out string originalValue)
            {
                value = Value;
                originalValue = OriginalValue;
            }

            public NormalizeFeatureStatus(string value, string originalValue)
            {
                Value = value;
                OriginalValue = originalValue;
            }

            public string Value { get; set; }
            public string OriginalValue { get; set; }
        }

        public static NormalizeFeatureStatus NormalizeFeature(this string featureValue, string featureName)
        {
            if (string.IsNullOrEmpty(featureName) || string.IsNullOrEmpty(featureValue))
                return new NormalizeFeatureStatus(featureValue, featureValue);

            switch (featureName)
            {
                case "سال ساخت":
                    var fixBuildYear = GlobalService.FixBuildYear(featureValue);
                    return new NormalizeFeatureStatus(fixBuildYear, featureValue);

                case "وام":
                case "قیمت نهایی":
                case "پیش پرداخت":
                case "اجاره":
                case "قیمت هر متر":
                    var pricePerMeterNormalized = featureValue.CleanNumberDividers();
                    return new NormalizeFeatureStatus(pricePerMeterNormalized, featureValue);

                default:
                    return new NormalizeFeatureStatus(featureValue, featureValue);
            }
        }

        public static (string, string, bool) NormalizeUiFeature(this string featureName, string featureValue)
        {
            if (string.IsNullOrEmpty(featureName) || string.IsNullOrEmpty(featureValue))
                return default;

            //var date = string.Empty;
            //if (log != null)
            //{
            //    var lastAudit = log?.Modifies?.Any() == true ? log.Modifies.LastOrDefault()?.DateTime : log.Create.DateTime;
            //    if (lastAudit != null)
            //    {
            //        date = $" — تا {((DateTime)lastAudit).GregorianToPersian(true)}";
            //    }
            //}

            switch (featureName)
            {
                case "سال ساخت":
                    if (!int.TryParse(featureValue, out var processedYear))
                        return new ValueTuple<string, string, bool>(featureName, featureValue, false);

                    var currentYear = new PersianCalendar().GetYear(DateTime.Now);
                    var finalYear = currentYear - processedYear;

                    var term = finalYear == 0 ? "نوساز" : $"{finalYear} سال ساخت";
                    return new ValueTuple<string, string, bool>(featureName, $"{term}", false);

                case "متراژ":
                    var isMeter = int.TryParse(featureValue, out var priceByMeter);
                    return new ValueTuple<string, string, bool>(featureName, isMeter ? $"{priceByMeter} متری" : featureValue, false);

                case "بر زمین":
                    return new ValueTuple<string, string, bool>(featureName, $"{featureValue} متر", false);

                case "پیش پرداخت":
                case "اجاره":
                case "وام":
                case "قیمت نهایی":
                    var words1 = featureValue.CurrencyToWords();

                    featureName = featureName.Equals("قیمت نهایی", StringComparison.CurrentCultureIgnoreCase) ? "قیمت" : featureName;
                    return new ValueTuple<string, string, bool>(featureName, $"{words1} تومان", true);

                case "قیمت هر متر":
                    var words2 = featureValue.CurrencyToWords();
                    return new ValueTuple<string, string, bool>(null, $"متری {words2} تومان", true);

                case "تعداد واحدهای مجتمع":
                    return new ValueTuple<string, string, bool>(featureName, $"{featureValue} واحده", false);

                case "تعداد خواب":
                    return new ValueTuple<string, string, bool>(featureName, $"{(featureValue == "1" ? "تک" : featureValue)} خوابه", false);

                default:
                    return new ValueTuple<string, string, bool>(featureName, featureValue, false);
            }
        }
    }
}