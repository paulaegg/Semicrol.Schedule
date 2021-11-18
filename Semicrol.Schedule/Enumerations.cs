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
            First,
            Second,
            Third,
            Fourth,
            Last
        }

        public enum AvailableWeekDays
        {
            Monday,
            Tuesday, 
            Wednesday, 
            Thursday,
            Friday, 
            Saturday, 
            Sunday, 
            Day,
            WeekDay,
            WeekendDay
        }
    }
}
