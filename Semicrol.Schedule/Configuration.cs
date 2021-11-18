using System;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class Configuration
    {
        public DateTime CurrentDate { get; set; }
        public ConfigurationTypes Type { get; set; }
        public bool Enabled { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? OnceExecutionTime { get; set; }
        public PeriodicityTypes Periodcity { get; set; }

        #region Weekly Configuration
        public int WeeklyPeriodicity { get; set; }
        public DayOfWeek[] WeeklyActiveDays { get; set; }
        #endregion

        #region Daily Configuration
        public ConfigurationTypes DailyType { get; set; }
        public TimeSpan DailyOnceTime { get; set; }
        public int DailyPeriodicity { get; set; }
        public TimePeriodicityTypes DailyPeriodicityType { get; set; }
        public TimeSpan DailyStartTime { get; set; }
        public TimeSpan DailyEndTime { get; set; } = new TimeSpan(23, 59, 58);
        #endregion

        #region Monthly Configuration
        public MonthlyTypes MonthlyType { get; set; }
        public int Day { get; set; }
        public int MonthlyPeriodicity { get; set; }
        public OrdinalPeriodicityTypes OrdinalPeriodicity { get; set; }
        public AvailableWeekDays WeekDay { get; set; }
        #endregion

    }
}