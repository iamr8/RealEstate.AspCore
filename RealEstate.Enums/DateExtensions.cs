using System;
using System.Globalization;

namespace RealEstate.Base
{
    public static class DateExtensions
    {
        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
            return dateTimeOffset.UtcDateTime;
        }

        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            try
            {
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
                var unixTimeSpan = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).ToLocalTime());
                return long.Parse(unixTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            }
            catch
            {
                return 0;
            }
        }

        public static DateTime PersianToGregorian(this string persianDate)
        {
            try
            {
                var userInput = persianDate;
                var userDateParts = userInput.Split(new[]
                {
                    "/", "-"
                }, StringSplitOptions.None);
                var userYear = int.Parse(userDateParts[0]);
                var userMonth = int.Parse(userDateParts[1]);
                var userDay = int.Parse(userDateParts[2]);

                var persianCalendar = new PersianCalendar();
                return persianCalendar.ToDateTime(userYear, userMonth, userDay, 0, 0, 0, 0);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static string GregorianToPersian(this DateTime dt, bool truncateTime, bool showSecs = false)
        {
            var pc = new PersianDateTime(dt)
            {
                ShowTime = !truncateTime,
                ShowTimeSecond = showSecs
            };

            var pdt = pc.ToString();
            return pdt;
        }
    }
}