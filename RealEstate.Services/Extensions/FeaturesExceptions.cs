using RealEstate.Base;
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

        public static FeatureException BuildYear = new FeatureException("cdb97926-b3b1-48ec-bdd6-389a0431007c", "سال ساخت", RegexPatterns.IranYear.GetDisplayName(), null, "فقط قالب سال : 1398");
        public static FeatureException ComplexUnits = new FeatureException("e75e0fb5-bdcd-470e-9e24-89a022d14490", "تعداد واحدهای مجتمع", @"^[\d]{1,2}", "واحده", "فقط اعداد");
        public static FeatureException Tenant = new FeatureException("03fb13b9-9a42-4e0f-a77c-08ed1a3bb179", "مستاجر", @"^[\u0600-\u06FF ]{1,} [\d]{1,}", null, "قالب مجاز : رضایی 09364091209 ( {نام} فاصله {شماره} )");
        public static FeatureException LoanPrice = new FeatureException("736ad605-78ea-41e1-bdeb-8d2811db2dec", "وام", @"^[\d]{1,}000000", "تومان", "فقط اعداد بر مبنای میلیون تومان");
        public static FeatureException GroundWidth = new FeatureException("93179c86-a0db-40cf-a0e2-d26c70a8de45", "بر زمین", @"^[\d]{1,3}", "متر", "فقط اعداد");

        public static FeatureException BedRooms = new FeatureException("b35f4bef-925e-415b-b8f1-19f6df02e6ac", "تعداد خواب", @"^[\d]{1}", "خوابه", "فقط اعداد");
        public static FeatureException AreaMeters = new FeatureException("15bf9d15-07bc-4f3c-8339-8192c8fd0c18", "متراژ", @"^[\d]{1,4}", "متر", "فقط اعداد");
        public static FeatureException Deposit = new FeatureException("22f68cda-29f2-4cc0-bb0f-e578defb85d1", "پیش پرداخت", @"^[\d]{1,}000000", "تومان", "فقط اعداد بر مبنای میلیون تومان");
        public static FeatureException FinalPrice = new FeatureException("54a0b920-c17f-4ff2-9c51-f9551159026a", "قیمت نهایی", @"^[\d]{1,}000000", "تومان", "فقط اعداد بر مبنای میلیون تومان");
        public static FeatureException PricePerMeter = new FeatureException("01cb6a1d-959d-4abb-8488-f10ab09bd8a8", "قیمت هر متر", @"^[\d]{1,}000000", "تومان", "فقط اعداد بر مبنای میلیون تومان");
        public static FeatureException RentPrice = new FeatureException("02cbebcc-610a-4bd2-8e27-e2d50b13587f", "کرایه", @"^[\d]{1,}000000", "تومان", "فقط اعداد بر مبنای میلیون تومان");
    }
}