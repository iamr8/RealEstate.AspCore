using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class EfFunctionExtensions
    {
        public static string Like(this string searchString)
        {
            return $"%{string.Join("%", searchString.Split(' ').ToArray())}%";
        }

        public static int IsNumeric(this string str)
        {
            return int.TryParse(str, out var num) ? num : 0;
        }
    }
}