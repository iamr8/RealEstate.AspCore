using System;

namespace RealEstate.Extensions
{
    public static class ArrayExtensions
    {
        public static int IndexOfNth(this string str, string value, int nth = 1)
        {
            if (nth <= 0)
                throw new ArgumentException("Can not find the zeroth index of substring in string. Must start with 1");
            var offset = str.IndexOf(value, StringComparison.Ordinal);
            for (var i = 1; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(value, offset + 1, StringComparison.Ordinal);
            }
            return offset;
        }
    }
}