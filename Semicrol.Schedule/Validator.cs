using System;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class Validator
    {
        private readonly Configuration _configuration;
        private readonly TranslationsManager _translationManager;

        public Validator(Configuration configuration, TranslationsManager translationManager)
        {
            this._configuration = configuration;
            this._translationManager = translationManager;
        }

        public void ValidateConfiguration()
        {
            this.DateValidation();
            this.LimitsValidation();
        }

        public void DateValidation()
        {
            if (_configuration.CurrentDate.IsValid() == false)
            {
                throw new Exception(_translationManager.GetText("currentCorrect"));
            }
            if (_configuration.StartDate.HasValue && _configuration.StartDate.Value.IsValid() == false)
            {
                throw new Exception(_translationManager.GetText("startDateCorrect"));
            }
            if (_configuration.EndDate.HasValue && _configuration.EndDate.Value.IsValid() == false)
            {
                throw new Exception(_translationManager.GetText("endDateCorrect"));
            }
        }

        public void LimitsValidation()
        {
            if (_configuration.StartDate.HasValue && _configuration.EndDate.HasValue &&
               _configuration.StartDate.Value > _configuration.EndDate.Value)
            {
                throw new Exception(_translationManager.GetText("endGreater"));
            }
        }

        public void ValidateRequiredConfigurationDate()
        {
            if (_configuration.Type == ConfigurationTypes.Once &&
                (_configuration.OnceExecutionTime.HasValue == false ||
                _configuration.OnceExecutionTime.Value.IsValid() == false))
            {
                throw new Exception(_translationManager.GetText("onceValidation"));
            }
        }

        public void ValidatePeriodicityConfiguration()
        {
            switch (_configuration.Periodcity)
            {
                case PeriodicityTypes.Daily:
                    break;
                case PeriodicityTypes.Weekly:
                    ValidateWeeklyConfiguration();
                    break;
                case PeriodicityTypes.Monthly:
                    break;
            }
        }

        public void ValidateWeeklyConfiguration()
        {
            if (_configuration.Periodcity != PeriodicityTypes.Weekly) { return; }

            if (_configuration.WeeklyPeriodicity <= 0 || _configuration.WeeklyPeriodicity.IsValid() == false)
            {
                throw new Exception(_translationManager.GetText("weeklyPeriodicityCorrect"));
            }
            if (_configuration.WeeklyActiveDays == null || _configuration.WeeklyActiveDays.Length == 0)
            {
                throw new Exception(_translationManager.GetText("weekDaysSelected"));
            }
        }

        public void ValidateDailyFrecuency()
        {
            if (_configuration.DailyType != ConfigurationTypes.Recurring) { return; }

            if (_configuration.DailyPeriodicity == 0 || _configuration.DailyPeriodicity.IsValid() == false)
            {
                throw new Exception("You should indicate a correct periodicity");
            }
            if (_configuration.DailyStartTime.IsValid() == false)
            {
                throw new Exception("Start Daily Frecuency should be a correct time");
            }
            if (_configuration.DailyEndTime.IsValid() == false || _configuration.DailyEndTime == TimeSpan.Zero)
            {
                throw new Exception("End Daily Frecuency should be a correct time distinct of zero");
            }
        }

        public void ValidateDailyOnceFrecuency()
        {
            if (_configuration.Type == ConfigurationTypes.Recurring &&
                _configuration.DailyType == ConfigurationTypes.Once &&
                _configuration.DailyOnceTime.IsValid() == false)
            {
                throw new Exception("The interval time in daily frecuency should be lower than 24 hours");
            }
        }

        public void ValidateCorrectDateWithCurrentDate(DateTime DateToValidate)
        {
            if (_configuration.CurrentDate > DateToValidate)
            {
                throw new Exception("Next execution time could not be lower than Current date");
            }
        }

        public void ValidateDateInLimits(DateTime DateToValidate)
        {
            if (_configuration.StartDate > DateToValidate ||
                _configuration.EndDate < DateToValidate)
            {
                throw new Exception("The date is out of the limits");
            }
        }

        private void ValidateMonthlyPeriodicity()
        {
            if (_configuration.MonthlyPeriodicity <= 0)
            {
                throw new Exception("You should enter a valid monthly periodicity");
            }
        }

        public void ValidateMonthlyConfigurationDayType()
        {
            if (_configuration.Periodcity != PeriodicityTypes.Monthly ||
                _configuration.MonthlyType != MonthlyTypes.Day)
            {
                return;
            }
            ValidateMonthlyPeriodicity();
            if (_configuration.MonthlyDay <= 0 ||
                _configuration.MonthlyDay > 31)
            {
                throw new Exception("You should enter a valid day");
            }           
        }

        public void ValidateMonthlyConfigurationBiuldType()
        {
            if (_configuration.Periodcity != PeriodicityTypes.Monthly ||
                _configuration.MonthlyType != MonthlyTypes.Built)
            {
                return;
            }
            ValidateMonthlyPeriodicity();
        }
    }
}