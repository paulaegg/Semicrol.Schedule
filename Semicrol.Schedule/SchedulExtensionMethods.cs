using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semicrol.Schedule
{
    public static class SchedulExtensionMethods
    {
        public static Boolean IsValid(this DateTime Date)
        {
            return Date != DateTime.MaxValue && Date != DateTime.MinValue;
        }

        public static Boolean IsValid(this int Number)
        {
            return int.TryParse(Number.ToString(), out int result);
        }

        public static Boolean IsValid(this TimeSpan Time)
        {
            return Time.TotalHours < 24 && Time.TotalHours >= 0;
        }

        public static DateTime[] FullWeek(this DateTime Date)
        {
            List<DateTime> WeekDays = new List<DateTime>();

            switch (Date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    WeekDays.Add(Date);
                    WeekDays.Add(Date.AddDays(1));
                    WeekDays.Add(Date.AddDays(2));
                    WeekDays.Add(Date.AddDays(3));
                    WeekDays.Add(Date.AddDays(4));
                    WeekDays.Add(Date.AddDays(5));
                    WeekDays.Add(Date.AddDays(6));
                    break;
                case DayOfWeek.Tuesday:
                    WeekDays.Add(Date.AddDays(-1));
                    WeekDays.Add(Date);
                    WeekDays.Add(Date.AddDays(1));
                    WeekDays.Add(Date.AddDays(2));
                    WeekDays.Add(Date.AddDays(3));
                    WeekDays.Add(Date.AddDays(4));
                    WeekDays.Add(Date.AddDays(5));
                    break;
                case DayOfWeek.Wednesday:
                    WeekDays.Add(Date.AddDays(-2));
                    WeekDays.Add(Date.AddDays(-1));
                    WeekDays.Add(Date);
                    WeekDays.Add(Date.AddDays(1));
                    WeekDays.Add(Date.AddDays(2));
                    WeekDays.Add(Date.AddDays(3));
                    WeekDays.Add(Date.AddDays(4));
                    break;
                case DayOfWeek.Thursday:
                    WeekDays.Add(Date.AddDays(-3));
                    WeekDays.Add(Date.AddDays(-2));
                    WeekDays.Add(Date.AddDays(-1));
                    WeekDays.Add(Date);
                    WeekDays.Add(Date.AddDays(1));
                    WeekDays.Add(Date.AddDays(2));
                    WeekDays.Add(Date.AddDays(3));
                    break;
                case DayOfWeek.Friday:
                    WeekDays.Add(Date.AddDays(-4));
                    WeekDays.Add(Date.AddDays(-3));
                    WeekDays.Add(Date.AddDays(-2));
                    WeekDays.Add(Date.AddDays(-1));
                    WeekDays.Add(Date);
                    WeekDays.Add(Date.AddDays(1));
                    WeekDays.Add(Date.AddDays(2));
                    break;
                case DayOfWeek.Saturday:
                    WeekDays.Add(Date.AddDays(-5));
                    WeekDays.Add(Date.AddDays(-4));
                    WeekDays.Add(Date.AddDays(-3));
                    WeekDays.Add(Date.AddDays(-2));
                    WeekDays.Add(Date.AddDays(-1));
                    WeekDays.Add(Date);
                    WeekDays.Add(Date.AddDays(1));
                    break;
                case DayOfWeek.Sunday:
                    WeekDays.Add(Date.AddDays(-6));
                    WeekDays.Add(Date.AddDays(-5));
                    WeekDays.Add(Date.AddDays(-4));
                    WeekDays.Add(Date.AddDays(-3));
                    WeekDays.Add(Date.AddDays(-2));
                    WeekDays.Add(Date.AddDays(-1));
                    WeekDays.Add(Date);
                    break;
            }
            return WeekDays.ToArray();
        }
        
        public static DateTime FullDateTime(this DateTime Day, TimeSpan Time)
        {
            return new DateTime(Day.Year, Day.Month, Day.Day, Time.Hours, Time.Minutes, Time.Seconds);
        }

        public static OutPut[] CalculateSerie(this Schedule schedule, int repeticions)
        {
            OutPut[] serie = new OutPut[repeticions];
            for (int index = 0; index < repeticions; index++)
            {
                serie[index] = schedule.GetNextExecution();
            }
            return serie;
        }
    }
}
