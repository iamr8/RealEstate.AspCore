using RealEstate.Base;
using RealEstate.Services.ServiceLayer;
using System;
using System.Linq;
using System.Reflection;

namespace RealEstate.Services.Extensions
{
    public class FeatureException
    {
        public FeatureException(string id, string name, string regularExpression, string unit, string message)
        {
            Id = id;
            Name = name;
            RegularExpression = regularExpression;
            Unit = unit;
            Message = message;
        }

        public FeatureException()
        {
        }

        public string Message { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string RegularExpression { get; set; }
        public string Unit { get; set; }
    }

    public static class FeaturesExceptions
    {
        public static FeatureException GetException(string name)
        {
            if (string.IsNullOrEmpty(name))
                return default;

            var exception = typeof(FeaturesExceptions)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(x => x.GetValue(null) as FeatureException)
                .FirstOrDefault(x => x?.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase) == true);
            return exception;
        }

        public static FeatureException BuildYear = new FeatureException(FeatureService.BuildYear, "سال ساخت", RegexPatterns.IranYear.GetDisplayName(), null, "فقط قالب سال : 1398");
        public static FeatureException ComplexUnits = new FeatureException(FeatureService.ComplexUnits, "تعداد واحدهای مجتمع", @"^[\d]{1,2}", "واحده", "فقط اعداد");
        public static FeatureException Tenant = new FeatureException(FeatureService.Tenant, "مستاجر", @"^[\u0600-\u06FF ]{1,} [\d]{1,}", null, "قالب مجاز : رضایی 09364091209 ( {نام} فاصله {شماره} )");
        public static FeatureException LoanPrice = new FeatureException(FeatureService.LoanPrice, "وام", @"^[\d]{1,}00000", "تومان", "فقط اعداد بر مبنای میلیون تومان");
        public static FeatureException GroundWidth = new FeatureException(FeatureService.GroundWidth, "بر زمین", @"^[\d]{1,3}", "متر", "فقط اعداد");

        public static FeatureException BedRooms = new FeatureException(FeatureService.BedRooms, "تعداد خواب", @"^[\d]{1}", "خوابه", "فقط اعداد");
        public static FeatureException AreaMeters = new FeatureException(FeatureService.Meterage, "متراژ", @"^[\d]{1,4}", "متر", "فقط اعداد");
        public static FeatureException Deposit = new FeatureException(FeatureService.DepositPrice, "پیش پرداخت", @"^[\d]{1,}00000", "تومان", "فقط اعداد بر مبنای میلیون تومان");
        public static FeatureException FinalPrice = new FeatureException(FeatureService.FinalPrice, "قیمت نهایی", @"^[\d]{1,}00000", "تومان", "فقط اعداد بر مبنای میلیون تومان");
        public static FeatureException PricePerMeter = new FeatureException(FeatureService.PricePerMeter, "قیمت هر متر", @"^[\d]{1,}00000", "تومان", "فقط اعداد بر مبنای میلیون تومان");
        public static FeatureException RentPrice = new FeatureException(FeatureService.RentPrice, "کرایه", @"^[\d]{1,}00000", "تومان", "فقط اعداد بر مبنای میلیون تومان");
    }
}