using Microsoft.EntityFrameworkCore;
using System;

namespace RealEstate.Services.Extensions
{
    public static class CustomDbFunctions
    {
        [DbFunction("JSON_VALUE", "")]
        public static string JsonValue(string source, string path) => throw new NotSupportedException();

        [DbFunction("DATEDIFF")]
        public static int DateDiff(string interval, string startingDate, string endingDate) => throw new NotSupportedException();

        [DbFunction("ISNUMERIC", "")]
        public static int IsNumeric(string str) => throw new NotSupportedException();

    }
}