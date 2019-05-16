using RealEstate.Base;
using System;
using System.Globalization;

namespace RealEstate.Services.Extensions
{
    public static class DateConverterExtension
    {
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