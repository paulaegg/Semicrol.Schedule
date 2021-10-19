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
            Action DateValidation = () => Validations.DateValidation(Configuration);
            Action ValidateConfiguration = () => Validations.ValidateConfiguration(Configuration);
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
                ConfigurationDate = new DateTime(2020, 1, 3),
                StartDate = DateTime.MinValue
            };
            Action DateValidation = () => Validations.DateValidation(Configuration);
            Action ValidateConfiguration = () => Validations.ValidateConfiguration(Configuration);
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
                ConfigurationDate = new DateTime(2020, 1, 3),
                EndDate = DateTime.MinValue
            };
            Action DateValidation = () => Validations.DateValidation(Configuration);
            Action ValidateConfiguration = () => Validations.ValidateConfiguration(Configuration);
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
            Action DateValidation = () => Validations.ValidateRequiredConfigurationDate(Configuration);
            DateValidation.Should().Throw<Exception>().WithMessage("If type is Once, you should enter a valid DateTime");
        }

        [Fact]
        public void validate_period()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationDate = DateTime.Today.AddDays(2),
                ConfigurationDays = -1
            };
            Action PeriodValidation = () => Validations.ValidateRequiredConfigurationDays(Configuration);
            PeriodValidation.Should().Throw<Exception>().WithMessage("If type is Recurrent, you should enter a valid configuration day");
        }

        [Fact]
        public void validate_limits()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationDate = new DateTime(2020, 1, 3),
                StartDate = new DateTime(2020, 1, 10),
                EndDate = new DateTime(2020, 1, 2)
            };
            Action LimitsValidation = () => Validations.LimitsValidation(Configuration.StartDate, Configuration.EndDate);
            LimitsValidation.Should().Throw<Exception>().WithMessage("End date should be greater than Start date");
        }

        [Fact]
        public void validate_configuration_once()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                ConfigurationType = ConfigurationTypes.Once,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationDate = new DateTime(2020, 1, 3),
                ConfigurationDays = 2,
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 10)
            };
            Action ValidateConfiguration = () => Validations.LimitsValidation(Configuration.StartDate, Configuration.EndDate);
            ValidateConfiguration.Should().NotThrow<Exception>();
        }

        [Fact]
        public void validate_configuration_recurring()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                ConfigurationType = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationDate = new DateTime(2020, 1, 3),
                ConfigurationDays = 2,
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 10)
            };
            Action ValidateConfiguration = () => Validations.LimitsValidation(Configuration.StartDate, Configuration.EndDate);
            ValidateConfiguration.Should().NotThrow<Exception>();
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
                ConfigurationType = ConfigurationTypes.Once,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationDate = new DateTime(2020, 1, 3),
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 10)
            };
            Schedule Schedule = new(Configuration);
            DateTime TheDate = Schedule.CalculateNextDate();
            TheDate.Should().Be(new DateTime(2020, 1, 3));
        }

        [Fact]
        public void validation_calculate_next_date_recurring()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                ConfigurationType = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationDays = 2,
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 10)
            };
            Schedule Schedule = new(Configuration);
            DateTime TheDate = Schedule.CalculateNextDate();
            TheDate.Should().Be(new DateTime(2020, 1, 3));
        }

        [Fact]
        public void validation_calculate_next_date_smaller_than_current_date()
        {
            Configuration Configuration = new()
            {
                CurrentDate = DateTime.Today,
            };
            Action ValidateCorrectDateWithCurrentDate = () => Validations.ValidateCorrectDateWithCurrentDate(Configuration,DateTime.Today.AddDays(-3));
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
            Action ValidateDateInLimits = () => Validations.ValidateDateInLimits(Configuration,DateTime.Today);
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
            Action ValidateDateInLimits = () => Validations.ValidateDateInLimits(Configuration, DateTime.Today.AddDays(20));
            ValidateDateInLimits.Should().Throw<Exception>().WithMessage("The date is out of the limits");
        }

        [Fact]
        public void validation_calculate_next_date_out_of_limits3()
        {
            Configuration Configuration = new()
            {
                StartDate = DateTime.Today.AddDays(10),
            };
            Action ValidateDateInLimits = () => Validations.ValidateDateInLimits(Configuration, DateTime.Today);
            ValidateDateInLimits.Should().Throw<Exception>().WithMessage("The date is out of the limits");
        }

        [Fact]
        public void validation_calculate_next_date_out_of_limits4()
        {
            Configuration Configuration = new()
            {
                StartDate = DateTime.Today.AddDays(1),
            };
            Action ValidateDateInLimits = () => Validations.ValidateDateInLimits(Configuration, DateTime.Today.AddDays(10));
            ValidateDateInLimits.Should().NotThrow<Exception>();
        }


        [Fact]
        public void validation_calculate_next_date_out_of_limits5()
        {
            Configuration Configuration = new()
            {
                EndDate = DateTime.Today.AddDays(2),
            };
            Action ValidateDateInLimits = () => Validations.ValidateDateInLimits(Configuration, DateTime.Today.AddDays(10));
            ValidateDateInLimits.Should().Throw<Exception>().WithMessage("The date is out of the limits");
        }

        [Fact]
        public void validation_calculate_next_date_out_of_limits6()
        {
            Configuration Configuration = new()
            {
                EndDate = DateTime.Today.AddDays(10),
            };
            Action ValidateDateInLimits = () => Validations.ValidateDateInLimits(Configuration, DateTime.Today.AddDays(2));
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
            Schedule.CalculateNextExecution().Description.Should().Be("The process is disabled");
        }

        #region Once 

        [Fact]
        public void validation_print_next_date_once1()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationType = ConfigurationTypes.Once,
                ConfigurationDate = new DateTime(2020, 1, 2, 8, 45, 0)
            };
            Schedule Schedule = new(Configuration);
            Schedule.CalculateDescription(Schedule.CalculateNextDate()).Should().Be("Occurs once. Schedule will be used on 02/01/2020 at 08:45");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 2, 8, 45, 0),
                Description = "Occurs once. Schedule will be used on 02/01/2020 at 08:45"
            };
            Schedule.CalculateNextExecution().Should().Be(NextExecutionOutPut);
        }

        [Fact]
        public void validation_print_next_date_once2()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationType = ConfigurationTypes.Once,
                ConfigurationDate = new DateTime(2020, 1, 6, 16, 30, 0),
                StartDate = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(Configuration);
            Schedule.CalculateDescription(Schedule.CalculateNextDate())
                .Should()
                .Be("Occurs once. Schedule will be used on 06/01/2020 at 16:30 starting on 02/01/2020");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 6, 16, 30, 0),
                Description = "Occurs once. Schedule will be used on 06/01/2020 at 16:30 starting on 02/01/2020"
            };
            Schedule.CalculateNextExecution().Should().Be(NextExecutionOutPut);
        }

        [Fact]
        public void validation_print_next_date_once3()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationType = ConfigurationTypes.Once,
                ConfigurationDate = new DateTime(2020, 1, 6, 16, 30, 0),
                EndDate = new DateTime(2020, 1, 20)
            };
            Schedule Schedule = new(Configuration);
            Schedule.CalculateDescription(Schedule.CalculateNextDate())
                .Should()
                .Be("Occurs once. Schedule will be used on 06/01/2020 at 16:30 ending on 20/01/2020");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 6, 16, 30, 0),
                Description = "Occurs once. Schedule will be used on 06/01/2020 at 16:30 ending on 20/01/2020"
            };
            Schedule.CalculateNextExecution().Should().Be(NextExecutionOutPut);
        }

        [Fact]
        public void validation_print_next_date_once4()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationType = ConfigurationTypes.Once,
                ConfigurationDate = new DateTime(2020, 1, 6, 16, 30, 0),
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 20)
            };
            Schedule Schedule = new(Configuration);
            Schedule.CalculateDescription(Schedule.CalculateNextDate())
                .Should()
                .Be("Occurs once. Schedule will be used on 06/01/2020 at 16:30 starting on 02/01/2020 and ending on 20/01/2020");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 6, 16, 30, 0),
                Description = "Occurs once. Schedule will be used on 06/01/2020 at 16:30 starting on 02/01/2020 and ending on 20/01/2020"
            };
            Schedule.CalculateNextExecution().Should().Be(NextExecutionOutPut);
        }

        #endregion

        #region Recurring
        [Fact]
        public void validation_print_next_date_recurring1()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationType = ConfigurationTypes.Recurring,
                ConfigurationDays = 6
            };
            Schedule Schedule = new(Configuration);
            Schedule.CalculateDescription(Schedule.CalculateNextDate())
                .Should()
                .Be("Occurs every day. Schedule will be used on 07/01/2020 at 00:00");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 7, 0, 0, 0),
                Description = "Occurs every day. Schedule will be used on 07/01/2020 at 00:00"
            };
            Schedule.CalculateNextExecution().Should().Be(NextExecutionOutPut);
        }
        [Fact]
        public void validation_print_next_date_recurring2()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationType = ConfigurationTypes.Recurring,
                ConfigurationDays = 6,
                StartDate = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(Configuration);
            Schedule.CalculateDescription(Schedule.CalculateNextDate())
                .Should()
                .Be("Occurs every day. Schedule will be used on 07/01/2020 at 00:00 starting on 02/01/2020");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 7, 0, 0, 0),
                Description = "Occurs every day. Schedule will be used on 07/01/2020 at 00:00 starting on 02/01/2020"
            };
            Schedule.CalculateNextExecution().Should().Be(NextExecutionOutPut);
        }
        [Fact]
        public void validation_print_next_date_recurring3()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationType = ConfigurationTypes.Recurring,
                ConfigurationDays = 6,
                EndDate = new DateTime(2020, 1, 20)
            };
            Schedule Schedule = new(Configuration);
            Schedule.CalculateDescription(Schedule.CalculateNextDate())
                .Should()
                .Be("Occurs every day. Schedule will be used on 07/01/2020 at 00:00 ending on 20/01/2020");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 7, 0, 0, 0),
                Description = "Occurs every day. Schedule will be used on 07/01/2020 at 00:00 ending on 20/01/2020"
            };
            Schedule.CalculateNextExecution().Should().Be(NextExecutionOutPut);
        }
        [Fact]
        public void validation_print_next_date_recurring4()
        {
            Configuration Configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                ConfigurationType = ConfigurationTypes.Recurring,
                ConfigurationDays = 6,
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 20)
            };
            Schedule Schedule = new(Configuration);
            Schedule.CalculateDescription(Schedule.CalculateNextDate())
                .Should()
                .Be("Occurs every day. Schedule will be used on 07/01/2020 at 00:00 starting on 02/01/2020 and ending on 20/01/2020");
            OutPut NextExecutionOutPut = new()
            {
                NextExecutionDate = new DateTime(2020, 1, 7, 0, 0, 0),
                Description = "Occurs every day. Schedule will be used on 07/01/2020 at 00:00 starting on 02/01/2020 and ending on 20/01/2020"
            };
            Schedule.CalculateNextExecution().Should().Be(NextExecutionOutPut);
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

    }
}
