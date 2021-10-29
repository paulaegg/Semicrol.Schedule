using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class Schedule
    {
        private readonly Configuration configuration;
        private Validator _validator;
        private DateTime? _firstActiveDay;

        public Schedule(Configuration Configuration)
        {
            this.configuration = Configuration ?? throw new NotImplementedException("You should define a configuration for the schedule");
        }

        private DateTime lastOutputDate { get; set; }

        private Validator validator
        {
            get
            {
                if (_validator == null)
                {
                    this._validator = new Validator(configuration);
                }
                return _validator;
            }
        }

        #region Description Texts
        private string textoOcurrs
        {
            get
            {
                return this.configuration.Type == ConfigurationTypes.Once ? "once" : $"every {textDailyConfig} {textHours}";
            }
        }

        private string textLimits
        {
            get
            {
                if (this.configuration.StartDate.HasValue == false &&
                    this.configuration.EndDate.HasValue == false)
                {
                    return string.Empty;
                }

                string Text = this.textStarLimit;
                Text += this.hasBothLimits ? " and " : string.Empty;
                Text += this.textEndLimit;

                return Text;
            }
        }

        private string textDailyConfig
        {
            get
            {
                if (configuration.Periodcity == PeriodicityType.Daily) { return "day"; }

                return $"{configuration.WeeklyPeriodicity} weeks on {textWeekDays}";
            }
        }

        private string textWeekDays
        {
            get
            {
                if (configuration.WeeklyActiveDays.Length == 0) { return string.Empty; }
                string text = configuration.WeeklyActiveDays.First().ToString();

                for (int index = 1; index < configuration.WeeklyActiveDays.Length -1; index++)
                {
                    text += ", " + configuration.WeeklyActiveDays[index].ToString();
                }

                text += " and " + configuration.WeeklyActiveDays.Last().ToString();
                return text;
            }
        }

        private string textHours
        {
            get
            {
                if (configuration.DailyType == ConfigurationTypes.Once)
                {
                    return $" at {configuration.DailyOnceTime}";
                }
                return $" every {configuration.DailyPeriodicity} {configuration.DailyPeriodicityType} between " +
                    $"{configuration.DailyStartTime} and {configuration.DailyEndTime} ";
                
            }
        }

        private bool hasBothLimits
        {
            get
            {
                return string.IsNullOrEmpty(this.textStarLimit) == false &&
                       string.IsNullOrEmpty(this.textEndLimit) == false;
            }
        }

        private string textStarLimit
        {
            get
            {
                return this.configuration.StartDate.HasValue
                    ? $"starting on {this.configuration.StartDate.Value:dd/MM/yyyy}"
                    : string.Empty;
            }
        }

        private string textEndLimit
        {
            get
            {
                return this.configuration.EndDate.HasValue
                    ? $"ending on { this.configuration.EndDate.Value:dd/MM/yyyy}"
                    : string.Empty;
            }
        }
        #endregion

        private DateTime firstActiveDay
        {
            get
            {
                if (this._firstActiveDay == null)
                {
                    this._firstActiveDay = this.GetFirstActiveDay();
                }
                return this._firstActiveDay.Value;
            }
        }

        public OutPut GetNextExecution()
        {
            if (this.configuration.Enabled == false)
            {
                return new OutPut() { Description = "The process is disabled" };
            }

            validator.ValidateConfiguration();
            lastOutputDate = this.GetNextDate();

            OutPut NextExecution = new OutPut()
            {
                NextExecutionDate = lastOutputDate,
                Description = GetDescription(lastOutputDate)
            };

            return NextExecution;
        }

        public DateTime GetNextDate()
        {
            DateTime NextDate = this.configuration.Type == ConfigurationTypes.Once
                ? this.GetNextDateOnceType()
                : this.GetNextRecurringExecution();

            validator.ValidateCorrectDateWithCurrentDate(NextDate);
            validator.ValidateDateInLimits(NextDate);
            
            return NextDate;
        }

        private DateTime GetNextDateOnceType()
        {
            validator.ValidateRequiredConfigurationDate();
            return this.configuration.OnceExecutionTime.Value;
        }

        #region Recurring Calculations

        private DateTime GetNextRecurringExecution()
        {
            this.validator.ValidateWeeklyConfiguration();
            DateTime NextDay = this.lastOutputDate.IsValid() == false ? firstActiveDay : lastOutputDate;

            DateTime? NextDate = this.CalculateTime(NextDay);
            if (NextDate.HasValue == false)
            {
                NextDay = this.GetNextDate(NextDay);
                NextDate = this.CalculateTime(NextDay);
            }
            return NextDate.Value;
        }

        private DateTime GetFirstActiveDay()
        {
            return configuration.Periodcity == PeriodicityType.Daily
                ? GetFirstActiveDayDaily()
                : GetFirstActiveDayWeekly();
        }

        private DateTime GetFirstActiveDayDaily()
        {
            return this.configuration.StartDate.HasValue
                 ? configuration.StartDate.Value
                 : this.configuration.CurrentDate;
        }

        private DateTime GetFirstActiveDayWeekly()
        {
            DateTime FirstActiveDate = this.configuration.StartDate.HasValue 
                ? configuration.StartDate.Value
                : this.configuration.CurrentDate;

            for (int days = 0; days < 7; days++)
            {
                if (this.configuration.WeeklyActiveDays.Contains(FirstActiveDate.DayOfWeek))
                {
                    break;
                }
                FirstActiveDate = FirstActiveDate.AddDays(1);
            }
            return FirstActiveDate;
        }



        private DateTime? CalculateTime(DateTime Day)
        {
            return this.configuration.DailyType == ConfigurationTypes.Once
                ? this.GetOnceTime(Day)
                : this.GetRecurringTime(Day);
        }

        private DateTime? GetOnceTime(DateTime Day)
        {
            validator.ValidateDailyOnceFrecuency();
            if (lastOutputDate == Day) { return null; }
            return Day.FullDateTime(this.configuration.DailyOnceTime);
        }

        private DateTime? GetRecurringTime(DateTime Day)
        {
            validator.ValidateDailyFrecuency();

            if (this.lastOutputDate.IsValid() == false || lastOutputDate.Date < Day.Date)
            {
                return Day.FullDateTime(this.configuration.DailyStartTime);
            }

            TimeSpan Time = Day.TimeOfDay.Add(this.configuration.DailyPeriodicityTime);
            Time = Time < this.configuration.DailyStartTime ? this.configuration.DailyStartTime : Time;

            if (Time.IsValid() == false ||
                Time > this.configuration.DailyEndTime)
            {
                return null;
            }

            return Day.FullDateTime(Time);
        }

        private DateTime GetNextDate(DateTime LastDate)
        {
            DateTime date = configuration.Periodcity == PeriodicityType.Daily
                ? LastDate.AddDays(1)
                : GetNextDateWeekly(LastDate);

            return date.FullDateTime(TimeSpan.Zero);
        }

        private DateTime GetNextDateWeekly(DateTime LastDate)
        {
            DateTime[] weekActiveDays = this.GetWeekActiveDays(LastDate);

            int IndexDay = Array.IndexOf(weekActiveDays, LastDate);
            if (IndexDay < configuration.WeeklyActiveDays.Length - 1)
            {
                return weekActiveDays[IndexDay + 1].FullDateTime(TimeSpan.Zero);
            }

            return weekActiveDays[0].Date.AddDays(this.configuration.WeeklyPeriodicityInDays);            
        }

        private DateTime[] GetWeekActiveDays(DateTime day)
        {
            return day.FullWeek()
                .Where(D => configuration.WeeklyActiveDays.Contains(D.DayOfWeek))
                .ToArray();
        }

        #endregion

        public string GetDescription(DateTime NextDate)
        {
            if (configuration.Type == ConfigurationTypes.Once)
            {
                return 
               $@"Occurs {this.textoOcurrs}. Schedule will be used on {NextDate:dd/MM/yyyy} at {NextDate:HH:mm} {this.textLimits}".Trim();
            }


            return $@"Occurs {this.textoOcurrs} {this.textLimits}".Trim(); ;
        }

    }
}