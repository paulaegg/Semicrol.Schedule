using System;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class Validator
    {
        Configuration configuration;

        public Validator(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void ValidateConfiguration()
        {
            this.DateValidation();
            this.LimitsValidation();
        }

        public void DateValidation()
        {
            if (configuration.CurrentDate.IsValid() == false)
            {
                throw new Exception("Current date should be a correct date");
            }
            if (configuration.StartDate.HasValue && configuration.StartDate.Value.IsValid() == false)
            {
                throw new Exception("Start Date should be a correct date");
            }
            if (configuration.EndDate.HasValue && configuration.EndDate.Value.IsValid() == false)
            {
                throw new Exception("End Date should be a correct date");
            }
        }

        public void LimitsValidation()
        {
            if (configuration.StartDate.HasValue && configuration.EndDate.HasValue &&
               configuration.StartDate.Value > configuration.EndDate.Value)
            {
                throw new Exception("End date should be greater than Start date");
            }
        }

        public void ValidateRequiredConfigurationDate()
        {
            if (configuration.Type == ConfigurationTypes.Once &&
                (configuration.OnceExecutionTime.HasValue == false ||
                configuration.OnceExecutionTime.Value.IsValid() == false))
            {
                throw new Exception("If type is Once, you should enter a valid DateTime");
            }
        }

        public void ValidateWeeklyConfiguration()
        {
            if (configuration.Periodcity != PeriodicityType.Weekly) { return; }

            if (configuration.WeeklyPeriodicity <= 0 || configuration.WeeklyPeriodicity.IsValid() == false)
            {
                throw new Exception("Weekly periodicity should be a correct number and greater than 0 if configuration occurs weekly");
            }
            if (configuration.WeeklyActiveDays == null || configuration.WeeklyActiveDays.Length == 0)
            {
                throw new Exception("You should select some day of the week if configuration occurs weekly");
            }
        }

        public void ValidateDailyFrecuency()
        {
            if (configuration.DailyType != ConfigurationTypes.Recurring) { return; }

            if (configuration.DailyPeriodicity == 0 || configuration.DailyPeriodicity.IsValid() == false)
            {
                throw new Exception("You should indicate a correct periodicity");
            }
            if (configuration.DailyStartTime.IsValid() == false)
            {
                throw new Exception("Start Daily Frecuency should be a correct time");
            }
            if (configuration.DailyEndTime.IsValid() == false || configuration.DailyEndTime == TimeSpan.Zero)
            {
                throw new Exception("End Daily Frecuency should be a correct time distinct of zero");
            }
        }

        public void ValidateDailyOnceFrecuency()
        {
            if (configuration.Type == ConfigurationTypes.Recurring &&
                configuration.DailyType == ConfigurationTypes.Once &&
                configuration.DailyOnceTime.IsValid() == false)
            {
                throw new Exception("The interval time in daily frecuency should be lower than 24 hours");
            }
        }

        public void ValidateCorrectDateWithCurrentDate(DateTime DateToValidate)
        {
            if (configuration.CurrentDate > DateToValidate)
            {
                throw new Exception("Next execution time could not be lower than Current date");
            }
        }

        public void ValidateDateInLimits(DateTime DateToValidate)
        {
            if (configuration.StartDate > DateToValidate ||
                configuration.EndDate < DateToValidate)
            {
                throw new Exception("The date is out of the limits");
            }
        }
    }
}