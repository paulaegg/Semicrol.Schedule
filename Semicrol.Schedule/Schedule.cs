using System;
using System.Globalization;
using System.Linq;
using System.Text;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class Schedule
    {
        private readonly Configuration _configuration;
        private Validator _validator;

        public Schedule(Configuration configuration)
        {
            _configuration = configuration ?? throw new Exception("You should define a configuration for the schedule");
        }

        private DateTime lastOutputDate { get; set; }


        #region Description Texts
        private string GetTextOcurrs()
        {
            return _configuration.Type == ConfigurationTypes.Once ? "once" : $"every {GetTextDailyConfig()}{GetTextHours()}";
        }

        private string GetTextLimits()
        {
            if (!_configuration.StartDate.HasValue &&
                !_configuration.EndDate.HasValue)
            {
                return string.Empty;
            }
            StringBuilder text = new StringBuilder(GetTextStarLimit());
            text.Append(CheckHasBothLimits() ? " and " : string.Empty);
            text.Append(GetTextEndLimit());

            return text.ToString();
        }

        private string GetTextDailyConfig()
        {
            switch (_configuration.Periodcity)
            {
                case PeriodicityTypes.Daily:
                    return "day";
                case PeriodicityTypes.Weekly:
                    return GetWeeklyPeriodicityText();
                case PeriodicityTypes.Monthly:
                    return string.Empty;
                default:
                    return string.Empty;
            }
            
        }

        private string GetWeeklyPeriodicityText()
        {
            if (_configuration.Periodcity != PeriodicityTypes.Weekly) { return string.Empty; }
            string weekText = _configuration.WeeklyPeriodicity == 1 ? "week" : "weeks";
            return $"{_configuration.WeeklyPeriodicity} {weekText} on {GetTextWeekDays()}";
        }

        private string GetTextWeekDays()
        {
            if (_configuration.WeeklyActiveDays.Length == 0) { return string.Empty; }

            StringBuilder text = new StringBuilder(_configuration.WeeklyActiveDays.First().ToString());
            for (int index = 1; index < _configuration.WeeklyActiveDays.Length - 1; index++)
            {
                text.Append(", " + _configuration.WeeklyActiveDays[index].ToString());
            }
            text.Append(" and " + _configuration.WeeklyActiveDays.Last().ToString());
            return text.ToString();
        }

        private string GetTextHours()
        {
            if (_configuration.DailyType == ConfigurationTypes.Once)
            {
                return $" at {_configuration.DailyOnceTime}";
            }
            return $" every {_configuration.DailyPeriodicity} {_configuration.DailyPeriodicityType} between " +
                $"{_configuration.DailyStartTime} and {_configuration.DailyEndTime}";
        }

        private bool CheckHasBothLimits()
        {
            return !string.IsNullOrEmpty(GetTextStarLimit()) &&
                   !string.IsNullOrEmpty(GetTextEndLimit());
        }

        private string GetTextStarLimit()
        {
            return _configuration.StartDate.HasValue
                ? $"starting on {_configuration.StartDate.Value.ToShortDateString()}"
                : string.Empty;
        }

        private string GetTextEndLimit()
        {
            return _configuration.EndDate.HasValue
                ? $"ending on { _configuration.EndDate.Value.ToShortDateString()}"
                : string.Empty;
        }

        #endregion

        private Validator GetValidator()
        {
            if (_validator == null)
            {
                _validator = new Validator(_configuration);
            }
            return _validator;
        }

        public OutPut GetNextExecution()
        {
            if (!_configuration.Enabled)
            {
                return new OutPut() { Description = "The process is disabled" };
            }

            GetValidator().ValidateConfiguration();
            lastOutputDate = GetNextDate();

            OutPut nextExecution = new OutPut()
            {
                NextExecutionDate = lastOutputDate,
                Description = GetDescription(lastOutputDate)
            };

            return nextExecution;
        }

        public DateTime GetNextDate()
        {
            DateTime nextDate = _configuration.Type == ConfigurationTypes.Once
                ? GetNextDateOnceType()
                : GetNextRecurringExecution();

            GetValidator().ValidateCorrectDateWithCurrentDate(nextDate);
            GetValidator().ValidateDateInLimits(nextDate);

            return nextDate;
        }

        private DateTime GetNextDateOnceType()
        {
            GetValidator().ValidateRequiredConfigurationDate();
            return _configuration.OnceExecutionTime.Value;
        }

        private DateTime GetNextRecurringExecution()
        {
            GetValidator().ValidatePeriodicityConfiguration();
            DateTime NextDay = lastOutputDate.IsValid() ? lastOutputDate : GetFirstActiveDay();

            DateTime? nextDate = CalculateTime(NextDay);
            if (!nextDate.HasValue)
            {
                NextDay = GetNextDate(NextDay);
                nextDate = CalculateTime(NextDay);
            }
            return nextDate.Value;
        }

        private DateTime GetFirstActiveDay()
        {
            switch (_configuration.Periodcity)
            {
                case PeriodicityTypes.Daily:
                    return GetFirstActiveDayDaily();
                case PeriodicityTypes.Weekly:
                    return GetFirstActiveDayWeekly();
                case PeriodicityTypes.Monthly:
                    return GetFirstActiveDayMonthly();
                default:
                    return lastOutputDate;
            }
        }

        private DateTime GetFirstActiveDayDaily()
        {
            return _configuration.StartDate ?? _configuration.CurrentDate;
        }

        private DateTime GetFirstActiveDayWeekly()
        {
            DateTime firstActiveDate = _configuration.StartDate ?? _configuration.CurrentDate;

            for (int days = 0; days < 7; days++)
            {
                if (_configuration.WeeklyActiveDays.Contains(firstActiveDate.DayOfWeek))
                {
                    break;
                }
                firstActiveDate = firstActiveDate.AddDays(1);
            }
            return firstActiveDate;
        }

        private DateTime GetFirstActiveDayMonthly()
        {
            return _configuration.MonthlyType == MonthlyTypes.Day
                ? GetFirstActiveDayMonthlyDayType()
                : GetFirstActiveDayMonthlyBiuldType();
        }

        private DateTime GetFirstActiveDayMonthlyDayType()
        {
            _validator.ValidateMonthliConfigurationDayType();

            DateTime firstActiveDate = _configuration.StartDate ?? _configuration.CurrentDate;

            if (firstActiveDate.Day > _configuration.MonthlyDay )
            {
                firstActiveDate = this.GetNextValidMonth(firstActiveDate);
            }

            return CheckLastMonthDay(firstActiveDate.Year, firstActiveDate.Month, _configuration.MonthlyDay);
        }

        private DateTime GetNextValidMonth(DateTime date)
        {
            do
            {
                date = date.AddMonths(1);
            }
            while (DateTime.DaysInMonth(date.Year, date.Month) < _configuration.MonthlyDay);

            return date;
        }
    
        private DateTime GetFirstActiveDayMonthlyBiuldType()
        {
            _validator.ValidateMonthliConfigurationBiuldType();
            //Si es Built mirar
            return DateTime.Today;
        }

        private DateTime? CalculateTime(DateTime day)
        {
            return _configuration.DailyType == ConfigurationTypes.Once
                ? GetOnceTime(day)
                : GetRecurringTime(day);
        }

        private DateTime? GetOnceTime(DateTime day)
        {
            GetValidator().ValidateDailyOnceFrecuency();
            if (lastOutputDate == day) { return null; }

            return day.FullDateTime(_configuration.DailyOnceTime);
        }

        private DateTime? GetRecurringTime(DateTime day)
        {
            GetValidator().ValidateDailyFrecuency();

            if (!lastOutputDate.IsValid() || lastOutputDate.Date < day.Date)
            {
                return day.FullDateTime(_configuration.DailyStartTime);
            }

            TimeSpan time = day.TimeOfDay.Add(GetDailyPeriodicityTime());
            time = time < _configuration.DailyStartTime ? _configuration.DailyStartTime : time;

            if (!time.IsValid() || time > _configuration.DailyEndTime)
            {
                return null;
            }

            return day.FullDateTime(time);
        }

        public TimeSpan GetDailyPeriodicityTime()
        {
            switch (_configuration.DailyPeriodicityType)
            {
                case TimePeriodicityTypes.Hours:
                    return new TimeSpan(_configuration.DailyPeriodicity, 0, 0);
                case TimePeriodicityTypes.Minutes:
                    return new TimeSpan(0, _configuration.DailyPeriodicity, 0);
                case TimePeriodicityTypes.Seconds:
                    return new TimeSpan(0, 0, _configuration.DailyPeriodicity);
                default:
                    return new TimeSpan(0, 0, 0);
            }
        }

        private DateTime GetNextDate(DateTime lastDate)
        {
            DateTime date = new DateTime();

            switch (_configuration.Periodcity)
            {
                case PeriodicityTypes.Daily:
                    date = lastDate.AddDays(1);
                    break;
                case PeriodicityTypes.Weekly:
                    date = GetNextDateWeekly(lastDate); 
                    break;
                case PeriodicityTypes.Monthly:
                    date = GetNextDateMonthly(lastDate);
                    break;
            }

            return date.FullDateTime(TimeSpan.Zero);
        }

        private DateTime GetNextDateWeekly(DateTime lastDate)
        {
            DateTime[] weekActiveDays = lastDate.ActiveWeekDays(_configuration.WeeklyActiveDays);

            int indexDay = Array.IndexOf(weekActiveDays, lastDate);
            if (indexDay < _configuration.WeeklyActiveDays.Length - 1)
            {
                return weekActiveDays[indexDay + 1].FullDateTime(TimeSpan.Zero);
            }

            return weekActiveDays[0].Date.AddDays(_configuration.WeeklyPeriodicity * 7);
        }

        private DateTime GetNextDateMonthly(DateTime lastDate)
        {
            if (_configuration.MonthlyType == MonthlyTypes.Day)
            {
                DateTime nextDate = lastDate.AddMonths(_configuration.MonthlyPeriodicity);
                return CheckLastMonthDay(nextDate.Year, nextDate.Month, nextDate.Day);
            }
            //Si es Built mirar
            return DateTime.Today;
        }

        private DateTime CheckLastMonthDay(int year, int month, int day)
        {
            if (day != _configuration.MonthlyDay ||
                day > DateTime.DaysInMonth(year, month))
            {
                day = _configuration.MonthlyDay <= DateTime.DaysInMonth(year, month)
                    ? _configuration.MonthlyDay
                    : DateTime.DaysInMonth(year, month);
            }
            return new DateTime(year, month, day);
        }

        public string GetDescription(DateTime nextDate)
        {
            if (_configuration.Type == ConfigurationTypes.Once)
            {
                return $@"Occurs {GetTextOcurrs()}. Schedule will be used on {nextDate.ToShortDateString()} at {nextDate.ToShortTimeString()} {GetTextLimits()}".Trim();
            }

            return $@"Occurs {GetTextOcurrs()} {GetTextLimits()}".Trim(); ;
        }
    }
}