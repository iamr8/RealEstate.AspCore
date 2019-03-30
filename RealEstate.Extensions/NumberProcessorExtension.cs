using System;

namespace RealEstate.Extensions
{
    public static class NumberProcessorExtension
    {
        public static int RoundToUp(double num)
        {
            return Convert.ToInt32(Math.Ceiling(num));
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