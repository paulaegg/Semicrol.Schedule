using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public static class Validations
    {
        public static Boolean IsCorrectDate(this DateTime Date)
        {
            return Date != DateTime.MaxValue && Date != DateTime.MinValue;
        }

        public static void ValidateConfiguration(Configuration configuration)
        {
            Validations.DateValidation(configuration);
            Validations.LimitsValidation(configuration.StartDate, configuration.EndDate);
        }

        public static void DateValidation(Configuration configuration)
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

        public static void ValidateRequiredConfigurationDays(Configuration configuration)
        {
            if (configuration.PeriodOccurs < TimeSpan.Zero)
            {
                throw new Exception("If type is Recurrent, you should enter a valid configuration day");
            }
        }

        public static void LimitsValidation(DateTime? StartDate, DateTime? EndDate)
        {
            if (StartDate.HasValue && EndDate.HasValue &&
               StartDate.Value > EndDate.Value)
            {
                throw new Exception("End date should be greater than Start date");
            }
        }

        public static void ValidateRequiredConfigurationDate(Configuration configuration)
        {
            if (configuration.ConfigurationDate.HasValue == false || configuration.ConfigurationDate.Value.IsCorrectDate() == false)
            {
                throw new Exception("If type is Once, you should enter a valid DateTime");
            }
        }

        public static void ValidateCorrectDateWithCurrentDate(Configuration configuration, DateTime DateToValidate)
        {
            if (configuration.CurrentDate > DateToValidate)
            {
                throw new Exception("Next execution time could not be greater than Current date");
            }
        }

        public static void ValidateDateInLimits(Configuration configuration, DateTime DateToValidate)
        {
            if (configuration.StartDate > DateToValidate ||
                configuration.EndDate < DateToValidate)
            {
                throw new Exception("The date is out of the limits");
            }
        }
    }
}