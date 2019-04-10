using System;

namespace RealEstate.Services.Extensions
{
    public static class NumberProcessorExtensions
    {
        public static int RoundToUp(double num)
        {
            return Convert.ToInt32(Math.Ceiling(num));
        }

        public static int FixPageNumber(this string pageNo)
        {
            var pg = string.IsNullOrEmpty(pageNo) ? 1 : int.TryParse(pageNo, out var page) ? page : 1;
            return pg;
        }

        public static string FixCurrency(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return default;

            return decimal.TryParse(text, out var currency) ? $"{currency:#,###}" : text;

        }

        public static string FixNumbers(this string num)
        {
            var result = "";
            var length = num.Length;
            if (length == 0)
                return num;

            for (var index = 0; index < length; ++index)
            {
                var character = num[index];
                if ('۰' <= character && character <= '۹')
                    character -= 'ۀ';
                result += character;
            }
            return result;
        }
    }
}