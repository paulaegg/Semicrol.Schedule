using System;
using Xunit;
using FluentAssertions;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule.Test
{
    public class ScheduleTest
    {

        #region Schedule Configuration
        [Fact]
        public void validate_current_date()
        {
            Configuration Configuration = new()
            {
                Enabled = true
            };
            Validator validator = new Validator(Configuration);
            Action DateValidation = () => validator.DateValidation();
            Action ValidateConfiguration = () => validator.ValidateConfiguration();
            DateValidation.Should().Throw<Exception>().WithMessage("Current date should be a correct date");
            ValidateConfiguration.Should().Throw<Exception>().WithMessage("Current date should be a correct date");
        }

        [Fact]
        public void validate_start_date()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = new DateTime(2020, 1, 3),
                StartDate = DateTime.MinValue
            };
            Validator validator = new Validator(Configuration);
            Action DateValidation = () => validator.DateValidation();
            Action ValidateConfiguration = () => validator.ValidateConfiguration();
            DateValidation.Should().Throw<Exception>().WithMessage("Start Date should be a correct date");
            ValidateConfiguration.Should().Throw<Exception>().WithMessage("Start Date should be a correct date");
        }

        [Fact]
        public void validate_end_date()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = new DateTime(2020, 1, 3),
                EndDate = DateTime.MinValue
            };
            Validator validator = new Validator(Configuration);
            Action DateValidation = () => validator.DateValidation();
            Action ValidateConfiguration = () => validator.ValidateConfiguration();
            DateValidation.Should().Throw<Exception>().WithMessage("End Date should be a correct date");
            ValidateConfiguration.Should().Throw<Exception>().WithMessage("End Date should be a correct date");
        }

        [Fact]
        public void validate_configuration_date_required()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1)
            };
            Validator validator = new Validator(Configuration);
            Action DateValidation = () => validator.ValidateRequiredConfigurationDate();
            DateValidation.Should().Throw<Exception>().WithMessage("If type is Once, you should enter a valid DateTime");
        }

        [Fact]
        public void validate_limits()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = new DateTime(2020, 1, 3),
                StartDate = new DateTime(2020, 1, 10),
                EndDate = new DateTime(2020, 1, 2)
            };
            Validator validator = new Validator(Configuration);
            Action LimitsValidation = () => validator.LimitsValidation();
            LimitsValidation.Should().Throw<Exception>().WithMessage("End date should be greater than Start date");
        }

        #endregion

        #region Schedule Calculate Next Date
        [Fact]
        public void validation_create_schedule()
        {
            Action CreateSchedule = () => new Schedule(null);
            CreateSchedule.Should().Throw<Exception>().WithMessage("You should define a configuration for the schedule");
        }

        [Fact]
        public void validation_calculate_next_date_once()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Once,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = new DateTime(2020, 1, 3),
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 10)
            };
            Schedule Schedule = new(Configuration);
            DateTime TheDate = Schedule.GetNextDate();
            TheDate.Should().Be(new DateTime(2020, 1, 3));
        }


        [Fact]
        public void validation_calculate_next_date_smaller_than_current_date()
        {
            Configuration Configuration = new()
            {
                CurrentDate = DateTime.Today,
            };
            Validator validator = new Validator(Configuration);
            Action ValidateCorrectDateWithCurrentDate = () => validator.ValidateCorrectDateWithCurrentDate(DateTime.Today.AddDays(-3));
            ValidateCorrectDateWithCurrentDate.Should().Throw<Exception>().WithMessage("Next execution time could not be greater than Current date");
        }

        [Fact]
        public void validation_calculate_next_date_out_of_limits1()
        {
            Configuration Configuration = new()
            {
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(20)
            };
            Validator validator = new Validator(Configuration);
            Action ValidateDateInLimits = () => validator.ValidateDateInLimits(DateTime.Today);
            ValidateDateInLimits.Should().Throw<Exception>().WithMessage("The date is out of the limits");
        }

        [Fact]
        public void validation_calculate_next_date_out_of_limits2()
        {
            Configuration Configuration = new()
            {
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2)
            };
            Validator validator = new Validator(Configuration);
            Action ValidateDateInLimits = () => validator.ValidateDateInLimits(DateTime.Today.AddDays(20));
            ValidateDateInLimits.Should().Throw<Exception>().WithMessage("The date is out of the limits");
        }

        [Fact]
        public void validation_calculate_next_date_out_of_limits3()
        {
            Configuration Configuration = new()
            {
                StartDate = DateTime.Today.AddDays(10),
            };
            Validator validator = new Validator(Configuration);
            Action ValidateDateInLimits = () => validator.ValidateDateInLimits(DateTime.Today);
            ValidateDateInLimits.Should().Throw<Exception>().WithMessage("The date is out of the limits");
        }

        [Fact]
        public void validation_calculate_next_date_out_of_limits4()
        {
            Configuration Configuration = new()
            {
                StartDate = DateTime.Today.AddDays(1),
            };
            Validator validator = new Validator(Configuration);
            Action ValidateDateInLimits = () => validator.ValidateDateInLimits(DateTime.Today.AddDays(10));
            ValidateDateInLimits.Should().NotThrow<Exception>();
        }

        [Fact]
        public void validation_calculate_next_date_out_of_limits5()
        {
            Configuration Configuration = new()
            {
                EndDate = DateTime.Today.AddDays(2),
            };
            Validator validator = new Validator(Configuration);
            Action ValidateDateInLimits = () => validator.ValidateDateInLimits(DateTime.Today.AddDays(10));
            ValidateDateInLimits.Should().Throw<Exception>().WithMessage("The date is out of the limits");
        }

        [Fact]
        public void validation_calculate_next_date_out_of_limits6()
        {
            Configuration Configuration = new()
            {
                EndDate = DateTime.Today.AddDays(10),
            };
            Validator validator = new Validator(Configuration);
            Action ValidateDateInLimits = () => validator.ValidateDateInLimits(DateTime.Today.AddDays(2));
            ValidateDateInLimits.Should().NotThrow<Exception>();
        }
        #endregion

        #region Schedule Print

        [Fact]
        public void validation_disabled()
        {
            Configuration Configuration = new()
            {
                Enabled = false
            };
            Schedule Schedule = new(Configuration);
            Schedule.GetNextExecution().Description.Should().Be("The process is disabled");
        }

        #region Once 

        [Fact]
        public void validation_print_next_date_once1()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Once,
                OnceExecutionTime = new DateTime(2020, 1, 2, 8, 45, 0)
            };
            Schedule Schedule = new(Configuration);
            Schedule.GetDescription(Schedule.GetNextDate()).Should().Be("Occurs once. Schedule will be used on 02/01/2020 at 08:45");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 2, 8, 45, 0),
                Description = "Occurs once. Schedule will be used on 02/01/2020 at 08:45"
            };
            Schedule.GetNextExecution().Should().Be(NextExecutionOutPut);
        }

        [Fact]
        public void validation_print_next_date_once2()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Once,
                OnceExecutionTime = new DateTime(2020, 1, 6, 16, 30, 0),
                StartDate = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(Configuration);
            Schedule.GetDescription(Schedule.GetNextDate())
                .Should()
                .Be("Occurs once. Schedule will be used on 06/01/2020 at 16:30 starting on 02/01/2020");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 6, 16, 30, 0),
                Description = "Occurs once. Schedule will be used on 06/01/2020 at 16:30 starting on 02/01/2020"
            };
            Schedule.GetNextExecution().Should().Be(NextExecutionOutPut);
        }

        [Fact]
        public void validation_print_next_date_once3()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Once,
                OnceExecutionTime = new DateTime(2020, 1, 6, 16, 30, 0),
                EndDate = new DateTime(2020, 1, 20)
            };
            Schedule Schedule = new(Configuration);
            Schedule.GetDescription(Schedule.GetNextDate())
                .Should()
                .Be("Occurs once. Schedule will be used on 06/01/2020 at 16:30 ending on 20/01/2020");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 6, 16, 30, 0),
                Description = "Occurs once. Schedule will be used on 06/01/2020 at 16:30 ending on 20/01/2020"
            };
            Schedule.GetNextExecution().Should().Be(NextExecutionOutPut);
        }

        [Fact]
        public void validation_print_next_date_once4()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Once,
                OnceExecutionTime = new DateTime(2020, 1, 6, 16, 30, 0),
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 20)
            };
            Schedule Schedule = new(Configuration);
            Schedule.GetDescription(Schedule.GetNextDate())
                .Should()
                .Be("Occurs once. Schedule will be used on 06/01/2020 at 16:30 starting on 02/01/2020 and ending on 20/01/2020");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 6, 16, 30, 0),
                Description = "Occurs once. Schedule will be used on 06/01/2020 at 16:30 starting on 02/01/2020 and ending on 20/01/2020"
            };
            Schedule.GetNextExecution().Should().Be(NextExecutionOutPut);
        }

        #endregion

        #endregion

        #region Validations Class
        [Fact]
        public void validation_is_correct_date()
        {
            DateTime TheDate = new DateTime(2020, 1, 1);
            TheDate.IsCorrectDate().Should().BeTrue();
        }
        [Fact]
        public void validation_is_incorrect_date()
        {
            DateTime TheDate = DateTime.MaxValue;
            TheDate.IsCorrectDate().Should().BeFalse();

            TheDate = DateTime.MinValue;
            TheDate.IsCorrectDate().Should().BeFalse();
        }

        #endregion

        #region Other Test
        [Fact]
        public void output_equals_null()
        {
            OutPut MyOutput = new()
            {
                NextExecutionDate = DateTime.Today,
                Description = string.Empty
            };
            MyOutput.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void output_equals_different_type()
        {
            OutPut MyOutput = new()
            {
                NextExecutionDate = DateTime.Today,
                Description = string.Empty
            };
            MyOutput.Equals(new object()).Should().BeFalse();
        }
        #endregion


        [Fact]
        public void validation_recurring_daily_once()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                Periodcity = PeriodicityType.Weekly,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                WeeklyPeriodicity = 2,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(10, 28, 4),
                StartDate = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(Configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 10, 28, 4));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 10, 28, 4));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 19, 10, 28, 4));
            result[3].NextExecutionDate.Should().Be(new DateTime(2020, 1, 27, 10, 28, 4));
            result[4].NextExecutionDate.Should().Be(new DateTime(2020, 2, 2, 10, 28, 4));
            result[5].NextExecutionDate.Should().Be(new DateTime(2020, 2, 10, 10, 28, 4));

            //result[5].Description.Should().Be(@"Occurs every 2 months on the fourth weekend in 2-hour periods between 2:00:00 and 4:00:00");
        }

        [Fact]
        public void validation_recurring_daily_recurring_hours()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                Periodcity = PeriodicityType.Weekly,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                WeeklyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityType.Hours,
                DailyStartTime = new TimeSpan(4, 0, 0),
                DailyEndTime = new TimeSpan(8, 0, 0),
                StartDate = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(Configuration);
            var result = Schedule.CalculateSerie(10);
            
            result.Length.Should().Be(10);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 4, 0, 0));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 6, 0, 0));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 8, 0, 0));
            result[3].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 0, 0));
            result[4].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 6, 0, 0));
            result[5].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 8, 0, 0));
            result[6].NextExecutionDate.Should().Be(new DateTime(2020, 1, 19, 4, 0, 0));
            result[7].NextExecutionDate.Should().Be(new DateTime(2020, 1, 19, 6, 0, 0));
            result[8].NextExecutionDate.Should().Be(new DateTime(2020, 1, 19, 8, 0, 0));
            result[9].NextExecutionDate.Should().Be(new DateTime(2020, 1, 27, 4, 0, 0));
            //result[9].Description.Should().Be(@"Occurs every 2 months on the fourth weekend in 2-hour periods between 2:00:00 and 4:00:00");
        }

        [Fact]
        public void validation_recurring_daily_recurring_mins()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                Periodcity = PeriodicityType.Weekly,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                WeeklyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityType.Minutes,
                DailyStartTime = new TimeSpan(4, 30, 0),
                DailyEndTime = new TimeSpan(4, 35, 0),
                StartDate = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(Configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 4, 30, 0));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 4, 32, 0));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 4, 34, 0));
            result[3].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 30, 0));
            result[4].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 32, 0));
            result[5].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 34, 0));
            result[6].NextExecutionDate.Should().Be(new DateTime(2020, 1, 19, 4, 30, 0));
        }

        [Fact]
        public void validation_recurring_daily_recurring_secs()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                Periodcity = PeriodicityType.Weekly,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                WeeklyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityType.Seconds,
                DailyStartTime = new TimeSpan(4, 30, 10),
                DailyEndTime = new TimeSpan(4, 30, 15),
                StartDate = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(Configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 4, 30, 10));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 4, 30, 12));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 4, 30, 14));
            result[3].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 30, 10));
            result[4].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 30, 12));
            result[5].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 30, 14));
            result[6].NextExecutionDate.Should().Be(new DateTime(2020, 1, 19, 4, 30, 10));
        }


    }
}
