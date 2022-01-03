  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public static class Description
    {
        public static string GetDescription(Configuration configuration, DateTime executionDate)
        {
            StringBuilder text = new StringBuilder("Occurs ");
            switch (configuration.Periodcity)   
            {
                case Enumerations.PeriodicityTypes.Daily:
                    text.Append(GetDescriptionOcursDaily(configuration));
                    text.Append(GetDescriptionDaily(configuration, executionDate));
                    break;
                case Enumerations.PeriodicityTypes.Weekly:
                    text.Append(GetDescriptionWeekly(configuration));
                    text.Append(GetDailyFrecuencyDescription(configuration));
                    break;
                case Enumerations.PeriodicityTypes.Monthly:
                    text.Append(GetDescriptionMonthly(configuration));
                    text.Append(GetDailyFrecuencyDescription(configuration));
                    break;
                default:
                    break;
            }
            text.Append(GetTextLimits(configuration));
            return text.ToString();
        }

        private static string GetDescriptionOcursDaily(Configuration configuration)
        {
            return configuration.Type == ConfigurationTypes.Recurring
                ? "every day"
                : "once";
        }

        private static string GetDescriptionDaily(Configuration configuration, DateTime executionDate)
        {
            if (configuration.DailyType == ConfigurationTypes.Once)
            {
                if (configuration.Type == ConfigurationTypes.Recurring)
                {
                    return $" at {configuration.DailyOnceTime}";
                }

                return $". Schedule will be used on {executionDate.ToShortDateString()} at {executionDate.ToShortTimeString()}";
            }
            return $" every {configuration.DailyPeriodicity} {configuration.DailyPeriodicityType} between " +
               $"{configuration.DailyStartTime} and {configuration.DailyEndTime}";

        }

        private static string GetDescriptionWeekly(Configuration configuration)
        {
            string weekText = configuration.WeeklyPeriodicity == 1 ? "week" : "weeks";
            return $"every {configuration.WeeklyPeriodicity} {weekText} on {GetTextWeekDays(configuration)}";
        }

        private static string GetTextWeekDays(Configuration configuration)
        {
            if (configuration.WeeklyActiveDays.Length == 0) { return string.Empty; }

            StringBuilder text = new StringBuilder(configuration.WeeklyActiveDays.First().ToString());
            if (configuration.WeeklyActiveDays.Length == 1) { return text.ToString(); }

            for (int index = 1; index < configuration.WeeklyActiveDays.Length - 1; index++)
            {
                text.Append(", " + configuration.WeeklyActiveDays[index].ToString());
            }
            text.Append(" and " + configuration.WeeklyActiveDays.Last().ToString());
            return text.ToString();
        }

        private static string GetDescriptionMonthly(Configuration configuration)
        {
            if (configuration.MonthlyType == MonthlyTypes.Day)
            {
                return $"the days {configuration.MonthlyDay} every {configuration.MonthlyPeriodicity} months";
            }
            return $"the {configuration.MonthlyOrdinalPeriodicity} {configuration.MonthlyWeekDay} of every {configuration.MonthlyPeriodicity} months";
        }

        private static string GetDailyFrecuencyDescription(Configuration configuration)
        {
            if (configuration.DailyType == ConfigurationTypes.Once)
            {
                return $" at {configuration.DailyOnceTime}";
            }
            return $" every {configuration.DailyPeriodicity} {configuration.DailyPeriodicityType} between " +
                $"{configuration.DailyStartTime} and {configuration.DailyEndTime}";
        }


        #region Limits text
        private static string GetTextLimits(Configuration configuration)
        {
            if (!configuration.StartDate.HasValue &&
                !configuration.EndDate.HasValue)
            {
                return string.Empty;
            }
            StringBuilder text = new StringBuilder(" ");
            text.Append(GetTextStarLimit(configuration));
            text.Append(CheckHasBothLimits(configuration) ? " and " : string.Empty);
            text.Append(GetTextEndLimit(configuration));

            return text.ToString();
        }

        private static bool CheckHasBothLimits(Configuration configuration)
        {
            return configuration.StartDate.HasValue &&
                   configuration.EndDate.HasValue;
        }

        private static string GetTextStarLimit(Configuration configuration)
        {
            return configuration.StartDate.HasValue
                ? $"starting on {configuration.StartDate.Value.ToShortDateString()}"
                : string.Empty;
        }

        private static string GetTextEndLimit(Configuration configuration)
        {
            return configuration.EndDate.HasValue
                ? $"ending on { configuration.EndDate.Value.ToShortDateString()}"
                : string.Empty;
        }
        #endregion
    }
}
