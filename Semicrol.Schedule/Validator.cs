using System;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class Validator
    {
        private readonly Configuration _configuration;
        private ResourceManager _resourceManager;

        public Validator(Configuration configuration, ResourceManager resourceManager)
        {
            _configuration = configuration;
            _resourceManager = resourceManager;
        }

        public void ValidateConfiguration()
        {
            DateValidation();
            LimitsValidation();
        }

        public void DateValidation()
        {
            if (_configuration.CurrentDate.IsValid() == false)
            {
                throw new Exception(_resourceManager.GetResource("currentCorrect"));
            }
            if (_configuration.StartDate.HasValue && _configuration.StartDate.Value.IsValid() == false)
            {
                throw new Exception(_resourceManager.GetResource("startCorrect"));
            }
            if (_configuration.EndDate.HasValue && _configuration.EndDate.Value.IsValid() == false)
            {
                throw new Exception(_resourceManager.GetResource("endCorrect"));
            }
        }

        public void LimitsValidation()
        {
            if (_configuration.StartDate.HasValue && _configuration.EndDate.HasValue &&
               _configuration.StartDate.Value > _configuration.EndDate.Value)
            {
                throw new Exception(_resourceManager.GetResource("endGreater"));
            }
        }

        public void ValidateRequiredConfigurationDate()
        {
            if (_configuration.Type == ConfigurationTypes.Once &&
                (_configuration.OnceExecutionTime.HasValue == false ||
                _configuration.OnceExecutionTime.Value.IsValid() == false))
            {
                throw new Exception(_resourceManager.GetResource("onceTypeCorrect"));
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
                throw new Exception(_resourceManager.GetResource("weeklyperiodicityValidation"));
            }
            if (_configuration.WeeklyActiveDays == null || _configuration.WeeklyActiveDays.Length == 0)
            {
                throw new Exception(_resourceManager.GetResource("daySelectedWeek"));
            }
        }
        
        public void ValidateDailyFrecuency()
        {
            if (_configuration.DailyType != ConfigurationTypes.Recurring) { return; }

            if (_configuration.DailyPeriodicity == 0 || _configuration.DailyPeriodicity.IsValid() == false)
            {
                throw new Exception(_resourceManager.GetResource("periodicityValidation"));
            }
            if (_configuration.DailyStartTime.IsValid() == false)
            {
                throw new Exception(_resourceManager.GetResource("startFrecuency"));
            }
            if (_configuration.DailyEndTime.IsValid() == false || _configuration.DailyEndTime == TimeSpan.Zero)
            {
                throw new Exception(_resourceManager.GetResource("endFrecuency"));
            }
        }

        public void ValidateDailyOnceFrecuency()
        {
            if (_configuration.Type == ConfigurationTypes.Recurring &&
                _configuration.DailyType == ConfigurationTypes.Once &&
                _configuration.DailyOnceTime.IsValid() == false)
            {
                throw new Exception(_resourceManager.GetResource("intervalTime"));
            }
        }

        public void ValidateCorrectDateWithCurrentDate(DateTime DateToValidate)
        {
            if (_configuration.CurrentDate > DateToValidate)
            {
                throw new Exception(_resourceManager.GetResource("nextExecution"));
            }
        }

        public void ValidateDateInLimits(DateTime DateToValidate)
        {
            if (_configuration.StartDate > DateToValidate ||
                _configuration.EndDate < DateToValidate)
            {
                throw new Exception(_resourceManager.GetResource("limitsValidation"));
            }
        }

        private void ValidateMonthlyPeriodicity()
        {
            if (_configuration.MonthlyPeriodicity <= 0)
            {
                throw new Exception(_resourceManager.GetResource("monthlyPeriodicity"));
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
                throw new Exception(_resourceManager.GetResource("validDay"));
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