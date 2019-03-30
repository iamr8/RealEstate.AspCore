using System.Linq;

namespace RealEstate.Extensions
{
    public static class EfFunctionExtension
    {
        public static string LikeExpression(this string searchString)
        {
            return $"%{string.Join("%", searchString.Split(' ').ToArray())}%";
        }
    }
}