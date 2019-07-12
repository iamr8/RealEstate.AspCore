using RealEstate.Base;
using RealEstate.Services.BaseLog;
using RealEstate.Services.ServiceLayer;
using System;
using System.Globalization;
using System.Linq;
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

        private static string CleanNumberDividers(this string num)
        {
            var featureValue = num.Replace(",", "").Replace("/", "").Replace(".", "").Replace("،", "");
            return featureValue;
        }

        public class NormalizeFeatureStatus
        {
            public NormalizeFeatureStatus()
            {
            }

            public NormalizeFeatureStatus(string value, string originalValue, bool hasError)
            {
                Value = value;
                HasError = hasError;
                OriginalValue = originalValue;
            }

            public string Value { get; set; }
            public bool HasError { get; set; }
            public string OriginalValue { get; set; }
        }

        public static NormalizeFeatureStatus NormalizeFeature(this string featureValue, string featureName)
        {
            if (string.IsNullOrEmpty(featureName) || string.IsNullOrEmpty(featureValue))
                return new NormalizeFeatureStatus(featureValue, featureValue, false);

            switch (featureName)
            {
                case "سال ساخت":

                    return new NormalizeFeatureStatus(GlobalService.FixBuildYear(featureValue), featureValue,
                        !Regex.IsMatch(featureValue, RegexPatterns.IranYear.GetDisplayName()));

                case "وام":
                    var normalizedLoanPrice = GlobalService.FixLoanPrice(featureValue);
                    return new NormalizeFeatureStatus(normalizedLoanPrice, featureValue, featureValue != normalizedLoanPrice);

                case "قیمت نهایی":
                    var normalizedFinalPrice = GlobalService.FixFinalPrice(featureValue);
                    return new NormalizeFeatureStatus(normalizedFinalPrice, featureValue, featureValue != normalizedFinalPrice);

                case "پیش پرداخت":
                case "اجاره":
                    featureValue = featureValue.CleanNumberDividers();
                    return new NormalizeFeatureStatus(featureValue, featureValue, false);

                case "قیمت هر متر":
                    featureValue = featureValue.CleanNumberDividers();
                    var pricePerMeterNormalized = long.TryParse(featureValue, out var pricePerMeter) && pricePerMeter > 30000000
                        ? $"{featureValue.Split('0')[0]}00000"
                        : featureValue;

                    return new NormalizeFeatureStatus(pricePerMeterNormalized, featureValue, featureValue != pricePerMeterNormalized);

                default:
                    return new NormalizeFeatureStatus(featureValue, featureValue, false);
            }
        }

        public static (string, string) NormalizeFeature(this string featureName, string featureValue, LogViewModel log, bool addDate = true)
        {
            if (string.IsNullOrEmpty(featureName) || string.IsNullOrEmpty(featureValue))
                return default;

            var date = string.Empty;
            if (log != null)
            {
                var lastAudit = log?.Modifies?.Any() == true ? log.Modifies.LastOrDefault()?.DateTime : log.Create.DateTime;
                if (lastAudit != null)
                {
                    date = $" — تا {((DateTime)lastAudit).GregorianToPersian(true)}";
                }
            }

            switch (featureName)
            {
                case "سال ساخت":
                    if (!int.TryParse(featureValue, out var processedYear))
                        return new ValueTuple<string, string>(featureName, featureValue);

                    var currentYear = new PersianCalendar().GetYear(DateTime.Now);
                    var finalYear = currentYear - processedYear;

                    var term = finalYear == 0 ? "نوساز" : $"{finalYear} سال";
                    return new ValueTuple<string, string>(featureName, $"{term}");

                case "متراژ":
                    var isMeter = int.TryParse(featureValue, out var priceByMeter);
                    return new ValueTuple<string, string>(featureName, isMeter ? $"{priceByMeter} متری" : featureValue);

                case "بر زمین":
                    return new ValueTuple<string, string>(featureName, $"{featureValue} متر");

                case "پیش پرداخت":
                case "اجاره":
                case "وام":
                case "قیمت نهایی":
                    var words1 = featureValue.CurrencyToWords();

                    featureName = featureName.Equals("قیمت نهایی", StringComparison.CurrentCultureIgnoreCase) ? "قیمت" : featureName;
                    return new ValueTuple<string, string>(featureName, $"{words1} تومان{(addDate ? date : "")}");

                case "قیمت هر متر":
                    var words2 = featureValue.CurrencyToWords();
                    return new ValueTuple<string, string>(null, $"متری {words2} تومان{(addDate ? date : "")}");

                case "تعداد واحدهای مجتمع":
                    return new ValueTuple<string, string>(featureName, $"{featureValue} واحده");

                case "تعداد خواب":
                    return new ValueTuple<string, string>(featureName, $"{(featureValue == "1" ? "تک" : featureValue)} خوابه");

                default:
                    return new ValueTuple<string, string>(featureName, featureValue);
            }
        }
    }
}