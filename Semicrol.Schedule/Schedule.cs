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
        private Configuration configuration;

        public Schedule(Configuration Configuration)
        {
            this.configuration = Configuration ?? throw new NotImplementedException("You should define a configuration for the schedule"); ;
        }

        #region Description Texts
        private string textoOcurrs
        {
            get
            {
                return this.configuration.ConfigurationType == ConfigurationTypes.Once ? "once" : "every day";
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

        public OutPut CalculateNextExecution()
        {
            if (this.configuration.Enabled == false)
            {
                return new OutPut() { Description = "The process is disabled" };
            }

            Validations.ValidateConfiguration(configuration);
            DateTime NextDate = this.CalculateNextDate();
           
            OutPut NextExecution = new OutPut()
            {
                NextExecutionDate = NextDate,
                Description = CalculateDescription(NextDate)
            };

            return NextExecution;
        }

        public DateTime CalculateNextDate()
        {
            DateTime NextDate = this.configuration.ConfigurationType == ConfigurationTypes.Once
                ? this.CalculateNextDateOnceType()
                : this.CalculateNextDateRecurringType();

            Validations.ValidateCorrectDateWithCurrentDate(this.configuration, NextDate);
            Validations.ValidateDateInLimits(this.configuration, NextDate);

            return NextDate;
        }

        private DateTime CalculateNextDateOnceType()
        {
            Validations.ValidateRequiredConfigurationDate(this.configuration);
            return this.configuration.ConfigurationDate.Value;
        }

        private DateTime CalculateNextDateRecurringType()
        {
            Validations.ValidateRequiredConfigurationDays(this.configuration);
            return this.configuration.CurrentDate + this.configuration.PeriodOccurs;
        }

        public string CalculateDescription(DateTime NextDate)
        {
            string description = 
                $@"Occurs {this.textoOcurrs}. Schedule will be used on {NextDate:dd/MM/yyyy} at {NextDate:HH:mm} {this.textLimits}";
            return description.Trim();
        }
    }
}
