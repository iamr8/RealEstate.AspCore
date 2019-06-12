using System;
using System.Globalization;
using System.Text;

namespace RealEstate.Base
{
    public class PersianDateTime
    {
        public int Hour { get; }
        public int Minute { get; }
        public int Year { get; }
        public int Month { get; }
        public int Second { get; }
        public int DayOfMonth { get; }
        public int MonthDays { get; set; }
        public bool ShowTime { get; set; } = false;
        public bool ShowTimeSecond { get; set; } = false;

        public int TotalSeconds => ToSeconds(Year, Month, DayOfMonth, Hour, Minute, Second);

        public DayOfWeek DayOfWeek { get; }

        public string DayOfWeekPersian
        {
            get
            {
                switch (DayOfWeek)
                {
                    case DayOfWeek.Friday:
                        return "جمعه";

                    case DayOfWeek.Monday:
                        return "دوشنبه";

                    case DayOfWeek.Sunday:
                        return "یکشنبه";

                    case DayOfWeek.Thursday:
                        return "پنجشنبه";

                    case DayOfWeek.Tuesday:
                        return "سه شنبه";

                    case DayOfWeek.Wednesday:
                        return "چهارشنبه";

                    case DayOfWeek.Saturday:
                    default:
                        return "شنبه";
                }
            }
        }

        public PersianDateTime(DateTime dateTime)
        {
            var persianCalendar = new PersianCalendar();

            Year = persianCalendar.GetYear(dateTime);
            Month = persianCalendar.GetMonth(dateTime);
            DayOfMonth = persianCalendar.GetDayOfMonth(dateTime);
            DayOfWeek = persianCalendar.GetDayOfWeek(dateTime);
            MonthDays = CountDayOfMonth(Month);

            Hour = dateTime.Hour;
            Minute = dateTime.Minute;
            Second = dateTime.Second;
        }

        private int CountDayOfMonth(int month)
        {
            try
            {
                var dayOfMonth = month > 0 && month < 7
                    ? 31
                    : month > 6 && month < 12
                        ? 30
                        : 29;

                return dayOfMonth;
            }
            catch
            {
                throw new ArgumentOutOfRangeException($"{nameof(Month)} is out of range. must be between 1 and 12");
            }
        }

        public int ToSeconds(int year, int month, int day, int hour, int minute, int second)
        {
            var b1 = second;
            const int min2Sec = 60;
            const int hr2Sec = min2Sec * 60;
            const int dy2Sec = hr2Sec * 24;
            var mnt2Sec = dy2Sec * CountDayOfMonth(month);
            var yr2Sec = mnt2Sec * 12;

            var b2 = b1 + (minute * min2Sec);
            var b3 = b2 + (hour * hr2Sec);
            var b4 = b3 + (day * dy2Sec);
            var b5 = b4 + (month * mnt2Sec);
            var b6 = b5 + (year * yr2Sec);

            return b6;
        }

        public string ToRelativeString()
        {
            var now = new PersianDateTime(DateTime.Now);

            var oneMinute = ToSeconds(0, 0, 0, 0, 1, 0);
            var tenMinutes = ToSeconds(0, 0, 0, 0, 10, 0);
            var halfHour = ToSeconds(0, 0, 0, 0, 30, 0);
            var quartHour = ToSeconds(0, 0, 0, 0, 45, 0);
            var oneHour = ToSeconds(0, 0, 0, 1, 0, 0);
            var oneDay = ToSeconds(0, 0, 1, 0, 0, 0);
            var oneWeek = ToSeconds(0, 0, 7, 0, 0, 0);
            var oneMonth = ToSeconds(0, 1, 0, 0, 0, 0);
            var oneYear = ToSeconds(1, 0, 0, 0, 0, 0);

            var division = now.TotalSeconds - TotalSeconds;
            if (division < 0)
                throw new IndexOutOfRangeException();

            var timeSpan = TimeSpan.FromSeconds(division);
            if (division < oneMinute) // زیر یک دقیقه
                return $"{division} ثانیه پیش";

            if (division >= oneMinute && division < tenMinutes) // بین 1 تا 10 دقیقه
                return $"{(int)timeSpan.TotalMinutes} دقیقه پیش";

            if (division >= tenMinutes && division < halfHour)
                return $"{((int)timeSpan.TotalMinutes % 5 == 0 ? (int)timeSpan.TotalMinutes : Math.Round(timeSpan.TotalMinutes / 5, 0) * 5)} دقیقه پیش";

            if (division >= halfHour && division < quartHour)
                return $"نیم ساعت پیش";

            if (division >= quartHour && division < oneHour) // ده دقیقه به بالا
                return $"کمتر از یک ساعت پیش";

            if (division >= oneHour && division < oneDay)
                return $"{(int)timeSpan.TotalHours} ساعت پیش";

            if (division >= oneDay && division < oneWeek)
                return $"{(int)timeSpan.TotalDays} روز پیش";

            if (division >= oneWeek && division < oneMonth)
                return $"{(int)timeSpan.TotalDays / 7} هفته پیش";

            if (division >= oneMonth && division < oneYear)
                return $"{(int)timeSpan.TotalDays / 30} ماه پیش";

            return ToString();
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool full, string dateSeparator = "/", string timeSeparator = ":")
        {
            var date = new StringBuilder();
            date.Append(full ? Year.ToString("####") : Year.ToString())
                .Append(dateSeparator)
                .Append(full ? Month.ToString("##") : Month.ToString())
                .Append(dateSeparator)
                .Append(full ? DayOfMonth.ToString("##") : DayOfMonth.ToString());

            var time = new StringBuilder();
            time.Append(full ? Hour.ToString("##") : Hour.ToString())
                .Append(timeSeparator)
                .Append(full ? Minute.ToString("##") : Minute.ToString());

            if (ShowTimeSecond)
            {
                time.Append(timeSeparator)
                    .Append(full ? Second.ToString("##") : Second.ToString());
            }

            return ShowTime ? $"{date} {time}" : $"{date}";
        }
    }
}