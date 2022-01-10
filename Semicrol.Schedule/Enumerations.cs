namespace Semicrol.Schedule
{
    public class Enumerations
    {
        public enum ConfigurationTypes
        {
            Once,
            Recurring
        }

        public enum PeriodicityTypes
        {
            Daily,
            Weekly, 
            Monthly
        }

        public enum TimePeriodicityTypes
        {
            Hours,
            Minutes,
            Seconds
        }

        public enum MonthlyTypes
        {
            Day,
            Built
        }

        public enum OrdinalPeriodicityTypes
        {
            First = 1,
            Second = 2,
            Third = 3,
            Fourth = 4,
            Last
        }

        public enum AvailableWeekDays
        {
            Monday = 1,
            Tuesday = 2, 
            Wednesday = 3, 
            Thursday = 4,
            Friday = 5, 
            Saturday = 6, 
            Sunday = 7, 
            Day,
            WeekDay,
            WeekendDay
        }

        public enum SupportingLanguages
        {
            es_ES,
            en_US,
            en_GB
        }
    }
}
