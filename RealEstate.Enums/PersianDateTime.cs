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

        public DayOfWeek DayOfWeek { get; }

        public PersianDateTime(DateTime dateTime)
        {
            var persianCalendar = new PersianCalendar();

            Year = persianCalendar.GetYear(dateTime);
            Month = persianCalendar.GetMonth(dateTime);
            DayOfMonth = persianCalendar.GetDayOfMonth(dateTime);
            DayOfWeek = persianCalendar.GetDayOfWeek(dateTime);

            if (Month > 0 && Month <= 6)
            {
                MonthDays = 31;
            }
            else if (Month > 6 && Month < 12)
            {
                MonthDays = 30;
            }
            else if (Month == 12)
            {
                MonthDays = 29;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"{nameof(Month)} is out of range. must be between 1 and 12");
            }

            Hour = persianCalendar.GetHour(dateTime);
            Minute = persianCalendar.GetMinute(dateTime);
            Second = persianCalendar.GetSecond(dateTime);
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