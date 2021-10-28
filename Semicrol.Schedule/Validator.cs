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
            if (configuration.CurrentDate.IsCorrectDate() == false)
            {
                throw new Exception("Current date should be a correct date");
            }          
            if (configuration.StartDate.HasValue && configuration.StartDate.Value.IsCorrectDate() == false)
            {
                throw new Exception("Start Date should be a correct date");
            }
            if (configuration.EndDate.HasValue && configuration.EndDate.Value.IsCorrectDate() == false)
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
            if (configuration.OnceExecutionTime.HasValue == false || configuration.OnceExecutionTime.Value.IsCorrectDate() == false)
            {
                throw new Exception("If type is Once, you should enter a valid DateTime");
            }
        }

        public void ValidateWeeklyConfiguration()
        {
            if (configuration.Periodcity != PeriodicityType.Weekly) { return; }

            if (configuration.WeeklyPeriodicity <= 0)
            {
                throw new Exception("Weekly periodicity should be greater than 0 if configuration occurs weekly");
            }
            if (configuration.WeeklyActiveDays == null)
            {
                throw new Exception("You should select some day of the week if configuration occurs weekly");
            }
        }

        public void ValidateDailyFrecuency()
        {
            if (configuration.DailyType == ConfigurationTypes.Recurring &&
                configuration.DailyPeriodicity == 0)
            {
                throw new Exception("You should indicate a periodicity");
            }
        }

        public void ValidateCorrectDateWithCurrentDate(DateTime DateToValidate)
        {
            if (configuration.CurrentDate > DateToValidate)
            {
                throw new Exception("Next execution time could not be greater than Current date");
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