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
        public PeriodicityType Periodcity { get; set; }

        #region Weekly Configuration
        public int WeeklyPeriodicity { get; set; }
        public DayOfWeek[] WeeklyActiveDays { get; set; }
        #endregion

        #region Daily Configuration
        public ConfigurationTypes DailyType { get; set; }
        public TimeSpan DailyOnceTime { get; set; }
        public int DailyPeriodicity { get; set; }
        public TimePeriodicityType DailyPeriodicityType { get; set; }
        public TimeSpan DailyStartTime { get; set; }
        public TimeSpan DailyEndTime { get; set; } = new TimeSpan(23, 59, 58);
        #endregion
    }
}