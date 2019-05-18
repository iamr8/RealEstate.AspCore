using System;
using System.Globalization;

namespace RealEstate.Services.KavenNegarProvider.Utils
{
    public class DateHelper
    {
        public static DateTime UnixTimestampToDateTime(long unixTimeStamp)
        {
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
            return dateTimeOffset.UtcDateTime;
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            try
            {
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour,
                    dateTime.Minute, dateTime.Second);
                var unixTimeSpan = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).ToLocalTime());
                return long.Parse(unixTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}