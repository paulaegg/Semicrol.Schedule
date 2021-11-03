using System;
using System.Globalization;
using System.Linq;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class Schedule
    {
        private readonly Configuration configuration;
        private Validator _validator;

        public Schedule(Configuration configuration)
        {
            this.configuration = configuration ?? throw new NotImplementedException("You should define a configuration for the schedule");
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
        private string GetTextOcurrs()
        {
            return this.configuration.Type == ConfigurationTypes.Once ? "once" : $"every {GetTextDailyConfig()}{GetTextHours()}";
        }

        private string GetTextLimits()
        {
            if (this.configuration.StartDate.HasValue == false &&
                this.configuration.EndDate.HasValue == false)
            {
                return string.Empty;
            }

            string Text = this.GetTextStarLimit();
            Text += this.CheckHasBothLimits() ? " and " : string.Empty;
            Text += this.GetTextEndLimit();

            return Text;
        }

        private string GetTextDailyConfig()
        {
            if (configuration.Periodcity == PeriodicityType.Daily) { return "day"; }
            string weekText = configuration.WeeklyPeriodicity == 1 ? "week" : "weeks";
            return $"{configuration.WeeklyPeriodicity} {weekText} on {GetTextWeekDays()}";
        }

        private string GetTextWeekDays()
        {
            if (configuration.WeeklyActiveDays.Length == 0) { return string.Empty; }
            string text = configuration.WeeklyActiveDays.First().ToString();

            for (int index = 1; index < configuration.WeeklyActiveDays.Length - 1; index++)
            {
                text += ", " + configuration.WeeklyActiveDays[index].ToString();
            }

            text += " and " + configuration.WeeklyActiveDays.Last().ToString();
            return text;
        }

        private string GetTextHours()
        {
            if (configuration.DailyType == ConfigurationTypes.Once)
            {
                return $" at {configuration.DailyOnceTime}";
            }
            return $" every {configuration.DailyPeriodicity} {configuration.DailyPeriodicityType} between " +
                $"{configuration.DailyStartTime} and {configuration.DailyEndTime}";
        }

        private bool CheckHasBothLimits()
        {
            return string.IsNullOrEmpty(this.GetTextStarLimit()) == false &&
                   string.IsNullOrEmpty(this.GetTextEndLimit()) == false;
        }

        private string GetTextStarLimit()
        {
            return this.configuration.StartDate.HasValue
                ? $"starting on {this.configuration.StartDate.Value:dd/MM/yyyy}"
                : string.Empty;
        }

        private string GetTextEndLimit()
        {
            return this.configuration.EndDate.HasValue
                ? $"ending on { this.configuration.EndDate.Value:dd/MM/yyyy}"
                : string.Empty;
        }

        #endregion

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

        private DateTime GetNextRecurringExecution()
        {
            this.validator.ValidateWeeklyConfiguration();
            DateTime NextDay = this.lastOutputDate.IsValid() == false ? GetFirstActiveDay() : lastOutputDate;

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

            TimeSpan Time = Day.TimeOfDay.Add(this.GetDailyPeriodicityTime());
            Time = Time < this.configuration.DailyStartTime ? this.configuration.DailyStartTime : Time;

            if (Time.IsValid() == false ||
                Time > this.configuration.DailyEndTime)
            {
                return null;
            }

            return Day.FullDateTime(Time);
        }

        public TimeSpan GetDailyPeriodicityTime()
        {
            switch (configuration.DailyPeriodicityType)
            {
                case TimePeriodicityType.Hours:
                    return new TimeSpan(configuration.DailyPeriodicity, 0, 0);
                case TimePeriodicityType.Minutes:
                    return new TimeSpan(0, configuration.DailyPeriodicity, 0);
                case TimePeriodicityType.Seconds:
                    return new TimeSpan(0, 0, configuration.DailyPeriodicity);
                default:
                    return new TimeSpan(0, 0, 0);
            }
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
            DateTime[] weekActiveDays = LastDate.ActiveWeekDays(configuration.WeeklyActiveDays);

            int IndexDay = Array.IndexOf(weekActiveDays, LastDate);
            if (IndexDay < configuration.WeeklyActiveDays.Length - 1)
            {
                return weekActiveDays[IndexDay + 1].FullDateTime(TimeSpan.Zero);
            }

            return weekActiveDays[0].Date.AddDays(this.configuration.WeeklyPeriodicity * 7);
        }
            
        public string GetDescription(DateTime NextDate)
        {
            if (configuration.Type == ConfigurationTypes.Once)
            {
                return
               $@"Occurs {this.GetTextOcurrs()}. Schedule will be used on {NextDate:dd/MM/yyyy} at {NextDate:HH:mm} {this.GetTextLimits()}".Trim();
            }

            return $@"Occurs {this.GetTextOcurrs()} {this.GetTextLimits()}".Trim(); ;
        }
    }
}