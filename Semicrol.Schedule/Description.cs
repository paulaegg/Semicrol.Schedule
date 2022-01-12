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
        public static string GetDescription(Configuration configuration, ResourceManager resourceManager, DateTime executionDate)
        {
            StringBuilder text = new StringBuilder(resourceManager.GetResource("occurs"));
            switch (configuration.Periodcity)   
            {
                case Enumerations.PeriodicityTypes.Daily:
                    text.Append(GetDescriptionOcursDaily(configuration, resourceManager));
                    text.Append(GetDescriptionDaily(configuration, resourceManager, executionDate));
                    break;
                case Enumerations.PeriodicityTypes.Weekly:
                    text.Append(GetDescriptionWeekly(configuration, resourceManager));
                    text.Append(GetDailyFrecuencyDescription(configuration, resourceManager));
                    break;
                case Enumerations.PeriodicityTypes.Monthly:
                    text.Append(GetDescriptionMonthly(configuration, resourceManager));
                    text.Append(GetDailyFrecuencyDescription(configuration, resourceManager));
                    break;
                default:
                    break;
            }
            text.Append(GetTextLimits(configuration, resourceManager));
            return text.ToString();
        }

        private static string GetDescriptionOcursDaily(Configuration configuration, ResourceManager resourceManager)
        {
            return configuration.Type == ConfigurationTypes.Recurring
                ? resourceManager.GetResource("everyday")
                : resourceManager.GetResource("once");
        }

        private static string GetDescriptionDaily(Configuration configuration, ResourceManager resourceManager, DateTime executionDate)
        {
            if (configuration.DailyType == ConfigurationTypes.Once)
            {
                if (configuration.Type == ConfigurationTypes.Recurring)
                {
                    return $" {resourceManager.GetResource("at")} {resourceManager.GetFormattedTime(configuration.DailyOnceTime)}";
                }

                return $"{resourceManager.GetResource("usedon")} {resourceManager.GetFormattedDate(executionDate)} {resourceManager.GetResource("at")} {resourceManager.GetFormattedTime(executionDate.TimeOfDay)}";
            }
            return $" {resourceManager.GetResource("every")} {configuration.DailyPeriodicity} {resourceManager.GetTimePeriodicityTranslated(configuration.DailyPeriodicityType)} {resourceManager.GetResource("between")} " +
               $"{resourceManager.GetFormattedTime(configuration.DailyStartTime)} {resourceManager.GetResource("and")} {resourceManager.GetFormattedTime(configuration.DailyEndTime)}";

        }

        private static string GetDescriptionWeekly(Configuration configuration, ResourceManager resourceManager)
        {
            string weekText = configuration.WeeklyPeriodicity == 1
                ? resourceManager.GetResource("week")
                : resourceManager.GetResource("weeks");
            return $"{resourceManager.GetResource("every")} {configuration.WeeklyPeriodicity} {weekText} {resourceManager.GetResource("on")} {GetTextWeekDays(configuration, resourceManager)}";
        }

        private static string GetTextWeekDays(Configuration configuration, ResourceManager resourceManager)
        {
            if (configuration.WeeklyActiveDays.Length == 0) { return string.Empty; }

            StringBuilder text = new StringBuilder(resourceManager.GetWeekDaysTranslated(configuration.WeeklyActiveDays.First()));
            if (configuration.WeeklyActiveDays.Length == 1) { return text.ToString(); }

            for (int index = 1; index < configuration.WeeklyActiveDays.Length - 1; index++)
            {
                text.Append(resourceManager.GetResource(",") + (resourceManager.GetWeekDaysTranslated(configuration.WeeklyActiveDays[index])));
            }
            text.Append(" " + resourceManager.GetResource("and") + " " + resourceManager.GetWeekDaysTranslated(configuration.WeeklyActiveDays.Last()));
            return text.ToString();
        }

        private static string GetDescriptionMonthly(Configuration configuration, ResourceManager resourceManager)
        {
            if (configuration.MonthlyType == MonthlyTypes.Day)
            {
                return $"{resourceManager.GetResource("thedays")} {configuration.MonthlyDay} {resourceManager.GetResource("every")} {configuration.MonthlyPeriodicity} {resourceManager.GetResource("months")}";
            }
            return $"{resourceManager.GetResource("the")} {resourceManager.GetOrdinalPeriodicityTranslated(configuration.MonthlyOrdinalPeriodicity)} {resourceManager.GetWeekDaysTranslated(configuration.MonthlyWeekDay)} {resourceManager.GetResource("ofevery")} {configuration.MonthlyPeriodicity} {resourceManager.GetResource("months")}";
        }

        private static string GetDailyFrecuencyDescription(Configuration configuration, ResourceManager resourceManager)
        {
            if (configuration.DailyType == ConfigurationTypes.Once)
            {
                return $" {resourceManager.GetResource("at")} {resourceManager.GetFormattedTime(configuration.DailyOnceTime)}";
            }
            return $" {resourceManager.GetResource("every")} {configuration.DailyPeriodicity} {resourceManager.GetTimePeriodicityTranslated(configuration.DailyPeriodicityType)} {resourceManager.GetResource("between")} " +
                $"{resourceManager.GetFormattedTime(configuration.DailyStartTime)} {resourceManager.GetResource("and")} {resourceManager.GetFormattedTime(configuration.DailyEndTime)}";
        }


        #region Limits text
        private static string GetTextLimits(Configuration configuration, ResourceManager resourceManager)
        {
            if (!configuration.StartDate.HasValue &&
                !configuration.EndDate.HasValue)
            {
                return string.Empty;
            }
            StringBuilder text = new StringBuilder(" ");
            text.Append(GetTextStarLimit(configuration, resourceManager));
            text.Append(CheckHasBothLimits(configuration) ? " "+ resourceManager.GetResource("and") +" " : string.Empty);
            text.Append(GetTextEndLimit(configuration, resourceManager));

            return text.ToString();
        }

        private static bool CheckHasBothLimits(Configuration configuration)
        {
            return configuration.StartDate.HasValue &&
                   configuration.EndDate.HasValue;
        }

        private static string GetTextStarLimit(Configuration configuration, ResourceManager resourceManager)
        {
            return configuration.StartDate.HasValue
                ? $"{resourceManager.GetResource("startingon")} {resourceManager.GetFormattedDate(configuration.StartDate.Value)}"
                : string.Empty;
        }

        private static string GetTextEndLimit(Configuration configuration, ResourceManager resourceManager)
        {
            return configuration.EndDate.HasValue
                ? $"{resourceManager.GetResource("endingon")} {resourceManager.GetFormattedDate(configuration.EndDate.Value)}"
                : string.Empty;
        }
        #endregion
    }
}
