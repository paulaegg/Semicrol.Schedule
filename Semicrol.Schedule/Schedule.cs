using System;
using System.Collections.Generic;
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
            DateTime firstActiveDate = GetFirstActiveDayDaily();

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

            DateTime firstActiveDate = GetFirstActiveDayDaily();

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
            
            return GetDayInMonth();
        }

        private DateTime GetDayInMonth()
        {
            return GetDayInMonth(new DateTime(GetFirstActiveDayDaily().Year, GetFirstActiveDayDaily().Month, 1));
        }

        private DateTime GetDayInMonth(DateTime date)
        {
            DateTime firstDayInMonth = new DateTime(date.Year, date.Month, 1);
           
            switch (this._configuration.MonthlyWeekDay)
            {
                case AvailableWeekDays.Day:
                    return date < GetFirstActiveDayDaily() ? GetFirstActiveDayDaily() : date;
                case AvailableWeekDays.WeekDay:
                    DayOfWeek[] weekDays = new DayOfWeek[5] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
                    return GetDayInMonthWeekDayOption(firstDayInMonth, weekDays);
                case AvailableWeekDays.WeekendDay:
                    DayOfWeek[] weekendDays = new DayOfWeek[2] { DayOfWeek.Saturday, DayOfWeek.Sunday };
                    return GetDayInMonthWeekDayOption(firstDayInMonth, weekendDays); ;
                default:
                    return GetDayInMonth(firstDayInMonth, (int)this._configuration.MonthlyWeekDay); 
            }
        }

        private DateTime GetDayInMonthWeekDayOption(DateTime firstDayInMonth, DayOfWeek[] weekDays)
        {
            List<DateTime> dates = new List<DateTime>();
            foreach (DayOfWeek day in weekDays)
            {
                DateTime date = GetDayInMonth(firstDayInMonth, (int)day);
                if (date.Month == firstDayInMonth.Month)
                {
                    if ((weekDays.Length == 2 && date.Day != 7) ||
                        (weekDays.Length == 5 && date.Day < 4))
                    {
                        return date;
                    }                    
                }
                dates.Add(date);
            }

            firstDayInMonth = firstDayInMonth.AddMonths(1);
            for (int i = 0; i < 3; i++)
            {
                if (weekDays.Contains(firstDayInMonth.DayOfWeek))
                {
                    return firstDayInMonth;
                }
                firstDayInMonth = firstDayInMonth.AddDays(1);
                i++;
            }
            return firstDayInMonth;
        }

        private DateTime GetDayInMonth(DateTime firstDayInMonth, int day)
        {
            int calculateDays = day - (int)firstDayInMonth.DayOfWeek;
            if (calculateDays < 0)
            {
                calculateDays += 7;
            }
            if (this._configuration.MonthlyOrdinalPeriodicity == OrdinalPeriodicityTypes.Last)
            {
                return GetLastWeekDayInMonth(firstDayInMonth.Year, firstDayInMonth.Month);
            }
            int searchedDay = (calculateDays + 1) + (7 * ((int)this._configuration.MonthlyOrdinalPeriodicity - 1));
            DateTime returnDate = new DateTime(firstDayInMonth.Year, firstDayInMonth.Month, searchedDay);

            if (returnDate < GetFirstActiveDayDaily())
            {
                returnDate = GetDayInMonth(firstDayInMonth.AddMonths(1), day);
            }
            return returnDate;
        }

        private DateTime GetLastWeekDayInMonth(int year, int month)
        {
            DateTime lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            int difference = (int)lastDay.DayOfWeek - (int)_configuration.MonthlyWeekDay;
            return difference > 0 
                ? lastDay.AddDays(-1 * difference) 
                : lastDay.AddDays(-1 * (7 + difference));

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
            DateTime nextDate = lastDate.AddMonths(_configuration.MonthlyPeriodicity);
            if (_configuration.MonthlyType == MonthlyTypes.Day)
            {
                return CheckLastMonthDay(nextDate.Year, nextDate.Month, nextDate.Day);
            }
            
            return this.GetDayInMonth(nextDate);
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
            return Description.GetDescription(_configuration, nextDate);
        }
    }
}