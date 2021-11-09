﻿using System;
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
            if (configuration == null)
            {
                throw new Exception("You should define a configuration for the schedule");
            }
            _configuration = configuration;
        }

        private DateTime lastOutputDate { get; set; }

        private Validator validator
        {
            get
            {
                if (_validator == null)
                {
                    _validator = new Validator(_configuration);
                }
                return _validator;
            }
        }

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
            if (_configuration.Periodcity == PeriodicityType.Daily) { return "day"; }
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

        public OutPut GetNextExecution()
        {
            if (!_configuration.Enabled)
            {
                return new OutPut() { Description = "The process is disabled" };
            }

            validator.ValidateConfiguration();
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

            validator.ValidateCorrectDateWithCurrentDate(nextDate);
            validator.ValidateDateInLimits(nextDate);

            return nextDate;
        }

        private DateTime GetNextDateOnceType()
        {
            validator.ValidateRequiredConfigurationDate();
            return _configuration.OnceExecutionTime.Value;
        }

        private DateTime GetNextRecurringExecution()
        {
            validator.ValidateWeeklyConfiguration();
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
            return _configuration.Periodcity == PeriodicityType.Daily
                ? GetFirstActiveDayDaily()
                : GetFirstActiveDayWeekly();
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

        private DateTime? CalculateTime(DateTime day)
        {
            return _configuration.DailyType == ConfigurationTypes.Once
                ? GetOnceTime(day)
                : GetRecurringTime(day);
        }

        private DateTime? GetOnceTime(DateTime day)
        {
            validator.ValidateDailyOnceFrecuency();
            if (lastOutputDate == day) { return null; }

            return day.FullDateTime(_configuration.DailyOnceTime);
        }

        private DateTime? GetRecurringTime(DateTime day)
        {
            validator.ValidateDailyFrecuency();

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
                case TimePeriodicityType.Hours:
                    return new TimeSpan(_configuration.DailyPeriodicity, 0, 0);
                case TimePeriodicityType.Minutes:
                    return new TimeSpan(0, _configuration.DailyPeriodicity, 0);
                case TimePeriodicityType.Seconds:
                    return new TimeSpan(0, 0, _configuration.DailyPeriodicity);
                default:
                    return new TimeSpan(0, 0, 0);
            }
        }

        private DateTime GetNextDate(DateTime lastDate)
        {
            DateTime date = _configuration.Periodcity == PeriodicityType.Daily
                ? lastDate.AddDays(1)
                : GetNextDateWeekly(lastDate);

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