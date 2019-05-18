using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Services.Extensions
{
    public static class SafeRandomExtension
    {
        public static T SelectRandom<T>(this List<T> list)
        {
            if (list.Count == 0) return default;

            var array = list.ToArray();
            return array[SelectRandom(0, array.Length - 1)];
        }

        public static T SelectRandom<T>(this IEnumerable<T> list)
        {
            var enumerable = list.ToList();
            if (enumerable.Count == 0) return default;

            var array = enumerable.ToArray();
            return array[SelectRandom(0, array.Length - 1)];
        }

        public static int SelectRandom(int min, int max)
        {
            var secureRandom = SecureRandom.GetInstance("Sha1PRNG");
            secureRandom.SetSeed(max);
            return (Math.Abs(secureRandom.NextInt()) % (max + 1)) + min;
        }
    }
}