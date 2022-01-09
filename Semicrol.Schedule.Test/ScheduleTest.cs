using FluentAssertions;
using System;
using Xunit;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule.Test
{
    public class ScheduleTest
    {
        #region Validator

        [Fact]
        public void Validator_Date_Validation_Current_Date()
        {
            Configuration configuration = new()
            {
                Enabled = true
            };
            Validator validator = new(configuration);
            Action DateValidation = () => validator.DateValidation();
            Action ValidateConfiguration = () => validator.ValidateConfiguration();
            DateValidation.Should().Throw<Exception>().WithMessage("Current date should be a correct date");
            ValidateConfiguration.Should().Throw<Exception>().WithMessage("Current date should be a correct date");
        }

        [Fact]
        public void Validator_Date_Validation_Start_Date()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = DateTime.Today,
                StartDate = DateTime.MaxValue
            };
            Validator validator = new(configuration);
            Action DateValidation = () => validator.DateValidation();
            Action ValidateConfiguration = () => validator.ValidateConfiguration();
            DateValidation.Should().Throw<Exception>().WithMessage("Start Date should be a correct date");
            ValidateConfiguration.Should().Throw<Exception>().WithMessage("Start Date should be a correct date");
        }

        [Fact]
        public void Validator_Date_Validation_End_Date()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = DateTime.Today,
                EndDate = DateTime.MinValue
            };
            Validator validator = new(configuration);
            Action DateValidation = () => validator.DateValidation();
            Action ValidateConfiguration = () => validator.ValidateConfiguration();
            DateValidation.Should().Throw<Exception>().WithMessage("End Date should be a correct date");
            ValidateConfiguration.Should().Throw<Exception>().WithMessage("End Date should be a correct date");
        }

        [Fact]
        public void Validator_Limits_Validation()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = new DateTime(2020, 1, 3),
                StartDate = new DateTime(2020, 1, 10),
                EndDate = new DateTime(2020, 1, 2)
            };
            Validator validator = new(configuration);
            Action LimitsValidation = () => validator.LimitsValidation();
            Action ValidateConfiguration = () => validator.ValidateConfiguration();
            LimitsValidation.Should().Throw<Exception>().WithMessage("End date should be greater than Start date");
            ValidateConfiguration.Should().Throw<Exception>().WithMessage("End date should be greater than Start date");
        }

        [Fact]
        public void Validator_Configuration_Correct()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2021, 1, 31)
            };
            Validator validator = new(configuration);
            Action ValidateConfiguration = () => validator.ValidateConfiguration();
            ValidateConfiguration.Should().NotThrow();
        }

        [Fact]
        public void Validator_Required_Configuration_Date()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Once,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = DateTime.MaxValue
            };
            Validator validator = new(configuration);
            Action ValidateRequiredConfigurationDate = () => validator.ValidateRequiredConfigurationDate();
            ValidateRequiredConfigurationDate.Should().Throw<Exception>().WithMessage("If type is Once, you should enter a valid DateTime");
        }

        [Fact]
        public void Validator_Required_Configuration_Date_Correct()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Once,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = new DateTime(2020, 1, 1)
            };
            Validator validator = new(configuration);
            Action ValidateRequiredConfigurationDate = () => validator.ValidateRequiredConfigurationDate();
            ValidateRequiredConfigurationDate.Should().NotThrow();
        }

        [Fact]
        public void Validator_Weekly_Configuration_Periodicity()
        {
            Configuration configuration = new()
            {
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = int.MinValue
            };
            Validator validator = new(configuration);
            Action ValidateWeeklyConfiguration = () => validator.ValidateWeeklyConfiguration();
            ValidateWeeklyConfiguration.Should().Throw<Exception>().WithMessage("Weekly periodicity should be a correct number and greater than 0 if configuration occurs weekly");
        }

        [Fact]
        public void Validator_Weekly_Configuration_Active_Days()
        {
            Configuration configuration = new()
            {
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = 25
            };
            Validator validator = new(configuration);
            Action ValidateWeeklyConfiguration = () => validator.ValidateWeeklyConfiguration();
            ValidateWeeklyConfiguration.Should().Throw<Exception>().WithMessage("You should select some day of the week if configuration occurs weekly");
        }

        [Fact]
        public void Validator_Weekly_Configuration_Correct()
        {
            Configuration configuration = new()
            {
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = 25,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday }
            };
            Validator validator = new(configuration);
            Action ValidateWeeklyConfiguration = () => validator.ValidateWeeklyConfiguration();
            ValidateWeeklyConfiguration.Should().NotThrow();
        }

        [Fact]
        public void Validator_Daily_Frecuency_Incorrect_Periodicity()
        {
            Configuration configuration = new()
            {
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 0
            };
            Validator validator = new(configuration);
            Action ValidateDailyFrecuency = () => validator.ValidateDailyFrecuency();
            ValidateDailyFrecuency.Should().Throw<Exception>().WithMessage("You should indicate a correct periodicity");
        }

        [Fact]
        public void Validator_Daily_Frecuency_Incorrect_Start_Time()
        {
            Configuration configuration = new()
            {
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 1,
                DailyStartTime = new TimeSpan(65, 0, 0)
            };
            Validator validator = new(configuration);
            Action ValidateDailyFrecuency = () => validator.ValidateDailyFrecuency();
            ValidateDailyFrecuency.Should().Throw<Exception>().WithMessage("Start Daily Frecuency should be a correct time");
        }

        [Fact]
        public void Validator_Daily_Frecuency_Incorrect_End_Time()
        {
            Configuration configuration = new()
            {
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 1,
                DailyEndTime = TimeSpan.Zero
            };
            Validator validator = new(configuration);
            Action ValidateDailyFrecuency = () => validator.ValidateDailyFrecuency();
            ValidateDailyFrecuency.Should().Throw<Exception>().WithMessage("End Daily Frecuency should be a correct time distinct of zero");
        }

        [Fact]
        public void Validator_Daily_Frecuency_Correct()
        {
            Configuration configuration = new()
            {
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 10
            };
            Validator validator = new(configuration);
            Action ValidateDailyFrecuency = () => validator.ValidateDailyFrecuency();
            ValidateDailyFrecuency.Should().NotThrow();
        }

        [Fact]
        public void Validator_Correct_Date_With_Current_Date()
        {
            Configuration configuration = new()
            {
                CurrentDate = new DateTime(2020, 1, 1)
            };
            Validator validator = new(configuration);
            Action ValidateCorrectDateWithCurrentDate = () => validator.ValidateCorrectDateWithCurrentDate(new DateTime(1993, 1, 15));
            ValidateCorrectDateWithCurrentDate.Should().Throw<Exception>().WithMessage("Next execution time could not be lower than Current date");
        }

        [Fact]
        public void Validator_Correct_Date_With_Current_Date_Correct()
        {
            Configuration configuration = new()
            {
                CurrentDate = new DateTime(2020, 1, 1)
            };
            Validator validator = new(configuration);
            Action ValidateCorrectDateWithCurrentDate = () => validator.ValidateCorrectDateWithCurrentDate(new DateTime(2021, 1, 15));
            ValidateCorrectDateWithCurrentDate.Should().NotThrow();
        }

        [Fact]
        public void Validator_Date_In_Limits_Lower()
        {
            Configuration configuration = new()
            {
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2020, 12, 31)
            };
            Validator validator = new(configuration);
            Action ValidateDateInLimits = () => validator.ValidateDateInLimits(new DateTime(1993, 1, 15));
            ValidateDateInLimits.Should().Throw<Exception>().WithMessage("The date is out of the limits");
        }

        [Fact]
        public void Validator_Date_In_Limits_Greater()
        {
            Configuration configuration = new()
            {
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2020, 12, 31)
            };
            Validator validator = new(configuration);
            Action ValidateDateInLimits = () => validator.ValidateDateInLimits(new DateTime(2023, 1, 15));
            ValidateDateInLimits.Should().Throw<Exception>().WithMessage("The date is out of the limits");
        }

        [Fact]
        public void Validator_Date_In_Limits_Correct()
        {
            Configuration configuration = new()
            {
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2020, 12, 31)
            };
            Validator validator = new(configuration);
            Action ValidateDateInLimits = () => validator.ValidateDateInLimits(new DateTime(2020, 1, 15));
            ValidateDateInLimits.Should().NotThrow();
        }

        [Fact]
        public void Validator_Daily_Once_Frecuency()
        {
            Configuration configuration = new()
            {
                Type = ConfigurationTypes.Recurring,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(26, 0, 0)
            };
            Validator validator = new(configuration);
            Action ValidateDailyOnceFrecuency = () => validator.ValidateDailyOnceFrecuency();
            ValidateDailyOnceFrecuency.Should().Throw<Exception>().WithMessage("The interval time in daily frecuency should be lower than 24 hours");
        }

        [Fact]
        public void Validate_Monthly_Configuration_Periodicity()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = DateTime.Today,
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Day,
                MonthlyDay = 1,
                MonthlyPeriodicity = -1
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("You should enter a valid monthly periodicity");
        }

        [Fact]
        public void Validate_Monthly_Configuration()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = DateTime.Today,
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Day,
                MonthlyDay = 55,
                MonthlyPeriodicity = 1
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("You should enter a valid day");
        }

        #endregion

        #region Schedule Basics

        [Fact]
        public void Create_Schedule_Null_Configuration()
        {
            Action CreateSchedule = () => new Schedule(null);
            CreateSchedule.Should().Throw<Exception>().WithMessage("You should define a configuration for the schedule");
        }

        [Fact]
        public void Disabled_Configuration()
        {
            Configuration configuration = new()
            {
                Enabled = false,
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(1);
            result[0].Description.Should().Be("The process is disabled");
        }

        [Fact]
        public void Calculate_Next_Execution_Incorrect_Current_Date()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Once,
                CurrentDate = DateTime.MinValue
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("Current date should be a correct date");
        }

        [Fact]
        public void Calculate_Next_Execution_Incorrect_Start_Date()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Once,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = new DateTime(2020, 1, 2),
                StartDate = DateTime.MaxValue
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("Start Date should be a correct date");
        }

        [Fact]
        public void Calculate_Next_Execution_Incorrect_End_Date()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = new DateTime(2020, 1, 2),
                EndDate = DateTime.MaxValue
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("End Date should be a correct date");
        }

        [Fact]
        public void Calculate_Next_Execution_Greater_Than_Current_Date()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Once,
                CurrentDate = new DateTime(2020, 1, 8),
                OnceExecutionTime = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("Next execution time could not be lower than Current date");
        }

        [Fact]
        public void Calculate_Next_Execution_Out_Of_Limits()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2019, 1, 1)
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("The date is out of the limits");
        }

        #endregion

        #region Schedule Type Once

        [Fact]
        public void Calculate_Next_Execution_Once_Type_Incorrect_Execution_Date()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Once,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = DateTime.MinValue
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("If type is Once, you should enter a valid DateTime");
        }

        [Fact]
        public void Calculate_Next_Execution_Once_Type_Correct()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Once,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = new DateTime(2020, 1, 3),
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 10)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(1);

            result.Length.Should().Be(1);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 3));
            result[0].Description.Should().Be("Occurs once. Schedule will be used on 03/01/2020 at 0:00 starting on 02/01/2020 and ending on 10/01/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Once_Type_Correct_Leap_Year()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Once,
                CurrentDate = new DateTime(2020, 1, 1),
                OnceExecutionTime = new DateTime(2020, 2, 29),
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 3, 1)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(1);

            result.Length.Should().Be(1);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 29));
            result[0].Description.Should().Be("Occurs once. Schedule will be used on 29/02/2020 at 0:00 starting on 02/01/2020 and ending on 01/03/2020");
        }

        #endregion

        #region Schedule Type Once Occurs Daily

        [Fact]
        public void Calculate_Next_Execution_Recurring_Daily_Once_Incorrect_Frecuency()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Daily,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(24, 0, 0)
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("The interval time in daily frecuency should be lower than 24 hours");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Daily_Once_Correct()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Daily,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(2, 0, 0)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(3);

            result.Length.Should().Be(3);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 1, 2, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 2, 2, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 3, 2, 0, 0));

            result[0].Description.Should().Be(@"Occurs every day at 02:00:00");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Daily_Incorrect_Periodicity()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Daily,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 0
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("You should indicate a correct periodicity");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Daily_Incorrect_Start_Time()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Daily,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 5,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(-2, 0, 0)
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("Start Daily Frecuency should be a correct time");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Daily_Incorrect_End_Time()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Daily,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 5,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyEndTime = TimeSpan.Zero
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("End Daily Frecuency should be a correct time distinct of zero");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Daily_Correct()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Daily,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 12,
                DailyPeriodicityType = TimePeriodicityTypes.Hours
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 1, 0, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 1, 12, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 2, 0, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 2, 12, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 3, 0, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 3, 12, 0, 0));

            result[0].Description.Should().Be(@"Occurs every day every 12 Hours between 00:00:00 and 23:59:58");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Daily_Leap_Year()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Daily,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 12,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                StartDate = new DateTime(2020, 2, 28)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 28, 0, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 28, 12, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 29, 0, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 29, 12, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 3, 1, 0, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 3, 1, 12, 0, 0));


            result[0].Description.Should().Be(@"Occurs every day every 12 Hours between 00:00:00 and 23:59:58 starting on 28/02/2020");
        }

        #endregion

        #region Schedule Type Once Occurs Weekly

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Incorrect_Weekly_Periodicity()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = -1

            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("Weekly periodicity should be a correct number and greater than 0 if configuration occurs weekly");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Incorrect_Weekly_Active_Days()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = 1,
                WeeklyActiveDays = new DayOfWeek[] { }
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("You should select some day of the week if configuration occurs weekly");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Correct1()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = 1,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Thursday }
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(3);

            result.Length.Should().Be(3);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 2, 0, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 6, 0, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 9, 0, 0, 0));

            result[0].Description.Should().Be(@"Occurs every 1 week on Monday and Thursday at 00:00:00");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Correct2()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = 1,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Friday },
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 12,
                DailyPeriodicityType = TimePeriodicityTypes.Hours
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 3, 0, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 3, 12, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 6, 0, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 6, 12, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 10, 0, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 10, 12, 0, 0));

            result[0].Description.Should().Be(@"Occurs every 1 week on Monday and Friday every 12 Hours between 00:00:00 and 23:59:58");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Correct3()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = 2,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Tuesday, DayOfWeek.Friday, DayOfWeek.Sunday },
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 12,
                DailyPeriodicityType = TimePeriodicityTypes.Hours
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(10);

            result.Length.Should().Be(10);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 3, 0, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 3, 12, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 5, 0, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 5, 12, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 14, 0, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 14, 12, 0, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 17, 0, 0, 0));
            result[7].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 17, 12, 0, 0));
            result[8].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 19, 0, 0, 0));
            result[9].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 19, 12, 0, 0));

            result[0].Description.Should().Be(@"Occurs every 2 weeks on Tuesday, Friday and Sunday every 12 Hours between 00:00:00 and 23:59:58");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Correct4()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = 3,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday },
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(2, 30, 0)

            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(10);

            result.Length.Should().Be(10);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 2, 2, 30, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 3, 2, 30, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 23, 2, 30, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 24, 2, 30, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 2, 13, 2, 30, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 2, 14, 2, 30, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 3, 6, 2, 30, 0));
            result[7].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 3, 7, 2, 30, 0));
            result[8].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 3, 27, 2, 30, 0));
            result[9].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 3, 28, 2, 30, 0));

            result[0].Description.Should().Be(@"Occurs every 3 weeks on Saturday and Sunday at 02:30:00");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Correct5()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 12, 15),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = 3,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(2, 30, 0),
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2021, 1, 31)

            };
            Schedule Schedule = new(configuration);

            Action GetNextExecution = () => Schedule.CalculateSerie(5);
            GetNextExecution.Should().Throw<Exception>().WithMessage("The date is out of the limits");

            var result = Schedule.CalculateSerie(4);

            result.Length.Should().Be(4);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 1, 2, 30, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 18, 2, 30, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 20, 2, 30, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 22, 2, 30, 0));

            result[0].Description.Should().Be(@"Occurs every 3 weeks on Monday, Wednesday and Friday at 02:30:00 starting on 01/01/2021 and ending on 31/01/2021");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Correct6()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2022, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = 1,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, 
                    DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday },
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(0, 0, 0),
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2022, 1, 31)

            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(8);

            result.Length.Should().Be(8);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 1, 0, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 2, 0, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 3, 0, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 4, 0, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 5, 0, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 6, 0, 0, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 7, 0, 0, 0));
            result[7].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 8, 0, 0, 0));

            result[0].Description.Should().Be(@"Occurs every 1 week on Monday, Tuesday, Wednesday, Thursday, Friday, Saturday and Sunday at 00:00:00 starting on 01/01/2021 and ending on 31/01/2022");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Correct_Leap_Year()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                CurrentDate = new DateTime(2020, 1, 1),
                Type = ConfigurationTypes.Recurring,
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyPeriodicity = 2,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Saturday },
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(2, 30, 0)

            };
            Schedule Schedule = new(configuration);

            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 4, 2, 30, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 18, 2, 30, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 1, 2, 30, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 15, 2, 30, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 29, 2, 30, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 3, 14, 2, 30, 0));

            result[0].Description.Should().Be(@"Occurs every 2 weeks on Saturday at 02:30:00");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Hours()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Saturday },
                WeeklyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(4, 0, 0),
                DailyEndTime = new TimeSpan(8, 0, 0),
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 31)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(10);

            result.Length.Should().Be(10);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 4, 4, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 4, 6, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 4, 8, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 13, 4, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 13, 6, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 13, 8, 0, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 18, 4, 0, 0));
            result[7].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 18, 6, 0, 0));
            result[8].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 18, 8, 0, 0));
            result[9].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 27, 4, 0, 0));
            result[0].Description.Should().Be("Occurs every 2 weeks on Monday and Saturday every 2 Hours between 04:00:00 and 08:00:00 starting on 02/01/2020 and ending on 31/01/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Minutes()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday },
                WeeklyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Minutes,
                DailyStartTime = new TimeSpan(4, 30, 0),
                DailyEndTime = new TimeSpan(4, 35, 0),
                StartDate = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 6, 4, 30, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 6, 4, 32, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 6, 4, 34, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 8, 4, 30, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 8, 4, 32, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 8, 4, 34, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 20, 4, 30, 0));
            result[0].Description.Should().Be("Occurs every 2 weeks on Monday and Wednesday every 2 Minutes between 04:30:00 and 04:35:00 starting on 02/01/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Seconds()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                Periodcity = PeriodicityTypes.Weekly,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Sunday },
                WeeklyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Seconds,
                DailyStartTime = new TimeSpan(4, 30, 10),
                DailyEndTime = new TimeSpan(4, 30, 15),
                StartDate = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 5, 4, 30, 10));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 5, 4, 30, 12));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 5, 4, 30, 14));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 13, 4, 30, 10));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 13, 4, 30, 12));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 13, 4, 30, 14));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 19, 4, 30, 10));

            result[0].Description.Should().Be(@"Occurs every 2 weeks on Monday and Sunday every 2 Seconds between 04:30:10 and 04:30:15 starting on 02/01/2020");
        }

        #endregion

        #region Schedule Type Occurs Monthly

        [Fact]
        public void Calculate_Next_Execution_Incorrect_Monthly_Periodicity()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 10, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Day,
                MonthlyDay = 15,
                MonthlyPeriodicity = -2
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("You should enter a valid monthly periodicity");
        }

        [Fact]
        public void Calculate_Next_Execution_Incorrect_Monthly_Day_Type()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 10, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Day,
                MonthlyDay = 32,
                MonthlyPeriodicity = 2
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.CalculateSerie(1);
            GetNextExecution.Should().Throw<Exception>().WithMessage("You should enter a valid day");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_DayType()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 10, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Day,
                MonthlyDay = 15,
                MonthlyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(2, 0, 0),
                DailyEndTime = new TimeSpan(4, 0, 0),
                StartDate = new DateTime(2020, 10, 2)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 15, 2, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 15, 4, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 15, 2, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 15, 4, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 2, 15, 2, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 2, 15, 4, 0, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 4, 15, 2, 0, 0));
            result[0].Description.Should().Be("Occurs the days 15 every 2 months every 2 Hours between 02:00:00 and 04:00:00 starting on 02/10/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_DayType2()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 10, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Day,
                MonthlyDay = 30,
                MonthlyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(2, 0, 0),
                DailyEndTime = new TimeSpan(4, 0, 0),
                StartDate = new DateTime(2020, 10, 2)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 30, 2, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 30, 4, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 30, 2, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 30, 4, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 2, 28, 2, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 2, 28, 4, 0, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 4, 30, 2, 0, 0));
            result[0].Description.Should().Be("Occurs the days 30 every 2 months every 2 Hours between 02:00:00 and 04:00:00 starting on 02/10/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_DayType3()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 9, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Day,
                MonthlyDay = 31,
                MonthlyPeriodicity = 3,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(2, 0, 0),
                DailyEndTime = new TimeSpan(4, 0, 0),
                StartDate = new DateTime(2020, 9, 2)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 9, 30, 2, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 9, 30, 4, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 31, 2, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 31, 4, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 3, 31, 2, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 3, 31, 4, 0, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 6, 30, 2, 0, 0));
            result[0].Description.Should().Be("Occurs the days 31 every 3 months every 2 Hours between 02:00:00 and 04:00:00 starting on 02/09/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_DayType4()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 9, 10),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Day,
                MonthlyDay = 5,
                MonthlyPeriodicity = 3,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(2, 0, 0),
                DailyEndTime = new TimeSpan(4, 0, 0),
                StartDate = new DateTime(2020, 9, 10)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 5, 2, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 5, 4, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 5, 2, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 5, 4, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 4, 5, 2, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 4, 5, 4, 0, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 7, 5, 2, 0, 0));
            result[0].Description.Should().Be("Occurs the days 5 every 3 months every 2 Hours between 02:00:00 and 04:00:00 starting on 10/09/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_DayType5()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2022, 10, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Day,
                MonthlyDay = 15,
                MonthlyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(2, 0, 0),
                DailyEndTime = new TimeSpan(4, 0, 0),
                StartDate = new DateTime(2020, 10, 2)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 10, 15, 2, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 10, 15, 4, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 12, 15, 2, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 12, 15, 4, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2023, 2, 15, 2, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2023, 2, 15, 4, 0, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2023, 4, 15, 2, 0, 0));
            result[0].Description.Should().Be("Occurs the days 15 every 2 months every 2 Hours between 02:00:00 and 04:00:00 starting on 02/10/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_DayType__Leap_Year()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Day,
                MonthlyDay = 29,
                MonthlyPeriodicity = 1,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(2, 0, 0),
                DailyEndTime = new TimeSpan(4, 0, 0)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 29, 2, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 29, 4, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 29, 2, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 29, 4, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 3, 29, 2, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 3, 29, 4, 0, 0));
            result[0].Description.Should().Be("Occurs the days 29 every 1 months every 2 Hours between 02:00:00 and 04:00:00");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 9, 10),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.First,
                MonthlyWeekDay = AvailableWeekDays.Wednesday,
                MonthlyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(2, 0, 0),
                DailyEndTime = new TimeSpan(4, 0, 0),
                StartDate = new DateTime(2020, 9, 10)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 7, 2, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 7, 4, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 2, 2, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 2, 4, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 2, 3, 2, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 2, 3, 4, 0, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 4, 7, 2, 0, 0));
            result[0].Description.Should().Be("Occurs the First Wednesday of every 2 months every 2 Hours between 02:00:00 and 04:00:00 starting on 10/09/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType2()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 9, 10),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.Fourth,
                MonthlyWeekDay = AvailableWeekDays.Monday,
                MonthlyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(2, 0, 0),
                DailyEndTime = new TimeSpan(4, 0, 0),
                StartDate = new DateTime(2020, 9, 10)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 9, 28, 2, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 9, 28, 4, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 11, 23, 2, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 11, 23, 4, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 25, 2, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 25, 4, 0, 0));
            result[6].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 3, 22, 2, 0, 0));
            result[0].Description.Should().Be("Occurs the Fourth Monday of every 2 months every 2 Hours between 02:00:00 and 04:00:00 starting on 10/09/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType3()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 9, 10),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.Second,
                MonthlyWeekDay = AvailableWeekDays.Tuesday,
                MonthlyPeriodicity = 1,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(12, 30, 0)
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(4);

            result.Length.Should().Be(4);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 13, 12, 30, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 11, 10, 12, 30, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 8, 12, 30, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 12, 12, 30, 0));
            result[0].Description.Should().Be("Occurs the Second Tuesday of every 1 months at 12:30:00");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType4()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 9, 10),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.Second,
                MonthlyWeekDay = AvailableWeekDays.Day,
                MonthlyPeriodicity = 1,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(12, 30, 0)
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(4);

            result.Length.Should().Be(4);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 9, 10, 12, 30, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 10, 12, 30, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 11, 10, 12, 30, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 10, 12, 30, 0));
            result[0].Description.Should().Be("Occurs the Second Day of every 1 months at 12:30:00");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType5()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 9, 10),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.Last,
                MonthlyWeekDay = AvailableWeekDays.Thursday,
                MonthlyPeriodicity = 2,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(12, 30, 0)
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(4);

            result.Length.Should().Be(4);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 9, 24, 12, 30, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 11, 26, 12, 30, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 28, 12, 30, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 3, 25, 12, 30, 0));
            result[0].Description.Should().Be("Occurs the Last Thursday of every 2 months at 12:30:00");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType6()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 9, 10),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.First,
                MonthlyWeekDay = AvailableWeekDays.WeekDay,
                MonthlyPeriodicity = 1,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(12, 30, 0)
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(4);

            result.Length.Should().Be(4);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 1, 12, 30, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 11, 2, 12, 30, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 1, 12, 30, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 1, 12, 30, 0));
            result[0].Description.Should().Be("Occurs the First WeekDay of every 1 months at 12:30:00");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType7()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 9, 10),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.First,
                MonthlyWeekDay = AvailableWeekDays.WeekendDay,
                MonthlyPeriodicity = 1,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(12, 30, 0)
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(4);

            result.Length.Should().Be(4);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 10, 3, 12, 30, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 11, 1, 12, 30, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 12, 5, 12, 30, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2021, 1, 2, 12, 30, 0));
            result[0].Description.Should().Be("Occurs the First WeekendDay of every 1 months at 12:30:00");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType8()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2021, 12, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.Third,
                MonthlyWeekDay = AvailableWeekDays.Friday,
                MonthlyPeriodicity = 4,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityTypes.Hours,
                DailyStartTime = new TimeSpan(10, 0, 0),
                DailyEndTime = new TimeSpan(12, 30, 15),
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 12, 31)
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 21, 10, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 21, 12, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 5, 20, 10, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 5, 20, 12, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 9, 16, 10, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 9, 16, 12, 0, 0));

            result[0].Description.Should().Be("Occurs the Third Friday of every 4 months every 2 Hours between 10:00:00 and 12:30:15 starting on 01/01/2022 and ending on 31/12/2022");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType9()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2021, 12, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.Last,
                MonthlyWeekDay = AvailableWeekDays.Saturday,
                MonthlyPeriodicity = 3,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 30,
                DailyPeriodicityType = TimePeriodicityTypes.Minutes,
                DailyStartTime = new TimeSpan(10, 0, 0),
                DailyEndTime = new TimeSpan(10, 30, 00),
                StartDate = new DateTime(2022, 2, 1),
                EndDate = new DateTime(2022, 12, 31)
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 2, 26, 10, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 2, 26, 10, 30, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 5, 28, 10, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 5, 28, 10, 30, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 8, 27, 10, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 8, 27, 10, 30, 0));


            result[0].Description.Should().Be("Occurs the Last Saturday of every 3 months every 30 Minutes between 10:00:00 and 10:30:00 starting on 01/02/2022 and ending on 31/12/2022");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType10()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2021, 12, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.Last,
                MonthlyWeekDay = AvailableWeekDays.Sunday,
                MonthlyPeriodicity = 6,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(0, 0, 0),
                StartDate = new DateTime(2022, 1, 1)
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 30, 0, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 7, 31, 0, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2023, 1, 29, 0, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2023, 7, 30, 0, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2024, 1, 28, 0, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2024, 7, 28, 0, 0, 0));

            result[0].Description.Should().Be("Occurs the Last Sunday of every 6 months at 00:00:00 starting on 01/01/2022");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_BuiltType11()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2022, 1, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.Last,
                MonthlyWeekDay = AvailableWeekDays.Sunday,
                MonthlyPeriodicity = 6,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(0, 0, 0),
                StartDate = new DateTime(2020, 1, 1)
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 1, 30, 0, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2022, 7, 31, 0, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2023, 1, 29, 0, 0, 0));
            result[3].NextExecutionDate.Should().BeSameDateAs(new DateTime(2023, 7, 30, 0, 0, 0));
            result[4].NextExecutionDate.Should().BeSameDateAs(new DateTime(2024, 1, 28, 0, 0, 0));
            result[5].NextExecutionDate.Should().BeSameDateAs(new DateTime(2024, 7, 28, 0, 0, 0));

            result[0].Description.Should().Be("Occurs the Last Sunday of every 6 months at 00:00:00 starting on 01/01/2020");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Monthly_Leap_Year()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                Periodcity = PeriodicityTypes.Monthly,
                MonthlyType = MonthlyTypes.Built,
                MonthlyOrdinalPeriodicity = OrdinalPeriodicityTypes.Last,
                MonthlyWeekDay = AvailableWeekDays.Saturday,
                MonthlyPeriodicity = 1,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(0, 0, 0)
            };

            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(3);

            result.Length.Should().Be(3);
            result[0].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 1, 25, 0, 0, 0));
            result[1].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 2, 29, 0, 0, 0));
            result[2].NextExecutionDate.Should().BeSameDateAs(new DateTime(2020, 3, 28, 0, 0, 0));

            result[0].Description.Should().Be("Occurs the Last Saturday of every 1 months at 00:00:00");
        }

        #endregion
    }
}