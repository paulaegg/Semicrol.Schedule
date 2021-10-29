using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class Configuration
    {
        public Configuration()
        { }

        public DateTime CurrentDate { get; set; }
        public ConfigurationTypes Type { get; set; }
        public bool Enabled { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? OnceExecutionTime { get; set; }
        public PeriodicityType Periodcity { get; set; }

        #region Weekly Configuration
        public int WeeklyPeriodicity { get; set; }
        public int WeeklyPeriodicityInDays
        {
            get
            {
                return WeeklyPeriodicity * 7;
            }
        }
        public DayOfWeek[] WeeklyActiveDays { get; set; }
        #endregion

        #region Daily Configuration
        public ConfigurationTypes DailyType { get; set; }
        public TimeSpan DailyOnceTime { get; set; }
        public int DailyPeriodicity { get; set; }
        public TimePeriodicityType DailyPeriodicityType { get; set; }
        public TimeSpan DailyPeriodicityTime
        {
            get
            {
                switch (DailyPeriodicityType)
                {
                    case TimePeriodicityType.Hours:
                        return new TimeSpan(DailyPeriodicity, 0, 0);
                    case TimePeriodicityType.Minutes:
                        return new TimeSpan(0, DailyPeriodicity, 0);
                    case TimePeriodicityType.Seconds:
                        return new TimeSpan(0, 0, DailyPeriodicity);
                    default:
                        return new TimeSpan(0, 0, 0);
                }
            }
        }
        public TimeSpan DailyStartTime { get; set; }
        public TimeSpan DailyEndTime { get; set; } = new TimeSpan(23, 59, 58);
        #endregion
    }
}