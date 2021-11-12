using System;
using System.Collections.Generic;
using System.Linq;

namespace Semicrol.Schedule
{
    public static class ScheduleExtensionMethods
    {
        public static Boolean IsValid(this DateTime date)
        {
            return date != DateTime.MaxValue && date != DateTime.MinValue;
        }

        public static Boolean IsValid(this int number)
        {
            return int.TryParse(number.ToString(), out _);
        }

        public static Boolean IsValid(this TimeSpan time)
        {
            return time.TotalHours < 24 && time.TotalHours >= 0;
        }

        public static DateTime[] ActiveWeekDays(this DateTime date, DayOfWeek[] weeklyActiveDays)
        {
            List<DateTime> weekDays = new List<DateTime>();
            int weekIndex = date.DayOfWeek == DayOfWeek.Sunday ? -6 : 1 - ((int)date.DayOfWeek);
            for (int index = 0; index < 7; index++)
            {
                weekDays.Add(date.AddDays(weekIndex++));
            }

            return weekDays.Where(D => weeklyActiveDays.Contains(D.DayOfWeek)).ToArray();
        }

        public static DateTime FullDateTime(this DateTime day, TimeSpan time)
        {
            return new DateTime(day.Year, day.Month, day.Day, time.Hours, time.Minutes, time.Seconds);
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