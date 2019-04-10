using System;
using System.Globalization;

namespace RealEstate.Services.Extensions
{
    public static class DateConverterExtension
    {
        public static DateTime PersianToGregorian(this string persianDate)
        {
            var userInput = persianDate;
            var userDateParts = userInput.Split(new[] { "/", "-" }, StringSplitOptions.None);
            var userYear = int.Parse(userDateParts[0]);
            var userMonth = int.Parse(userDateParts[1]);
            var userDay = int.Parse(userDateParts[2]);

            var persianCalendar = new PersianCalendar();
            return persianCalendar.ToDateTime(userYear, userMonth, userDay, 0, 0, 0, 0);
        }

        public static string GregorianToPersian(this DateTime dt, bool truncateTime, bool showSecs = false)
        {
            var persianCalendar = new PersianCalendar();
            var time = string.Empty;

            var year = persianCalendar.GetYear(dt).ToString("0000");
            var month = persianCalendar.GetMonth(dt).ToString("00");
            var day = persianCalendar.GetDayOfMonth(dt).ToString("00");
            var date = $"{year}/{month}/{day}";

            if (truncateTime) return $"{date}{time}";

            var hour = persianCalendar.GetHour(dt).ToString("00");
            var minute = persianCalendar.GetMinute(dt).ToString("00");

            var sec = string.Empty;
            if (showSecs)
                sec = $":{persianCalendar.GetSecond(dt):00}";

            time = $" {hour}:{minute}{sec}";
            return $"{date}{time}";
        }
    }
}