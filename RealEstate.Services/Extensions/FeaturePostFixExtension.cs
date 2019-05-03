namespace RealEstate.Services.Extensions
{
    public static class FeaturePostFixExtension
    {
        public static string Postfix(this string featureName)
        {
            if (string.IsNullOrEmpty(featureName))
                return default;

            string postfix;
            if (featureName.Contains("قیمت"))
            {
                postfix = "تومان";
            }
            else if (featureName.Equals("متراژ"))
            {
                postfix = "متر";
            }
            else if (featureName.Equals("بر زمین"))
            {
                postfix = "متر";
            }
            else
            {
                postfix = null;
            }

            return " " + postfix;
        }
    }
}