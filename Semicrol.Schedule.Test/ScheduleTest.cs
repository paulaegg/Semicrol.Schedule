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
                Periodcity = PeriodicityType.Weekly,
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
                Periodcity = PeriodicityType.Weekly,
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
                Periodcity = PeriodicityType.Weekly,
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
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 3));
            result[0].Description.Should().Be("Occurs once. Schedule will be used on 03/01/2020 at 00:00 starting on 02/01/2020 and ending on 10/01/2020");
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
                Periodcity = PeriodicityType.Daily,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(24, 0, 0)
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
                Periodcity = PeriodicityType.Daily,
                DailyType = ConfigurationTypes.Once,
                DailyOnceTime = new TimeSpan(2, 0, 0)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(3);

            result.Length.Should().Be(3);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 1, 2, 0, 0));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 2, 2, 0, 0));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 3, 2, 0, 0));

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
                Periodcity = PeriodicityType.Daily,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 0
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
                Periodcity = PeriodicityType.Daily,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 5,
                DailyPeriodicityType = TimePeriodicityType.Hours,
                DailyStartTime = new TimeSpan(-2, 0, 0)
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
                Periodcity = PeriodicityType.Daily,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 5,
                DailyPeriodicityType = TimePeriodicityType.Hours,
                DailyEndTime = TimeSpan.Zero
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
                Periodcity = PeriodicityType.Daily,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 12,
                DailyPeriodicityType = TimePeriodicityType.Hours
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 1, 0, 0, 0));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 1, 12, 0, 0));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 2, 0, 0, 0));
            result[3].NextExecutionDate.Should().Be(new DateTime(2020, 1, 2, 12, 0, 0));
            result[4].NextExecutionDate.Should().Be(new DateTime(2020, 1, 3, 0, 0, 0));
            result[5].NextExecutionDate.Should().Be(new DateTime(2020, 1, 3, 12, 0, 0));

            result[0].Description.Should().Be(@"Occurs every day every 12 Hours between 00:00:00 and 23:59:58");
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
                Periodcity = PeriodicityType.Weekly,
                WeeklyPeriodicity = -1

            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
                Periodcity = PeriodicityType.Weekly,
                WeeklyPeriodicity = 1,
                WeeklyActiveDays = new DayOfWeek[] { }
            };
            Schedule Schedule = new(configuration);
            Action GetNextExecution = () => Schedule.GetNextExecution();
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
                Periodcity = PeriodicityType.Weekly,
                WeeklyPeriodicity = 1,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Thursday }
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(3);

            result.Length.Should().Be(3);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 2, 0, 0, 0));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 6, 0, 0, 0));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 9, 0, 0, 0));

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
                Periodcity = PeriodicityType.Weekly,
                WeeklyPeriodicity = 1,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Friday },
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 12,
                DailyPeriodicityType = TimePeriodicityType.Hours
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(6);

            result.Length.Should().Be(6);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 3, 0, 0, 0));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 3, 12, 0, 0));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 6, 0, 0, 0));
            result[3].NextExecutionDate.Should().Be(new DateTime(2020, 1, 6, 12, 0, 0));
            result[4].NextExecutionDate.Should().Be(new DateTime(2020, 1, 10, 0, 0, 0));
            result[5].NextExecutionDate.Should().Be(new DateTime(2020, 1, 10, 12, 0, 0));

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
                Periodcity = PeriodicityType.Weekly,
                WeeklyPeriodicity = 2,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Tuesday, DayOfWeek.Friday, DayOfWeek.Sunday },
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 12,
                DailyPeriodicityType = TimePeriodicityType.Hours
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(10);

            result.Length.Should().Be(10);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 3, 0, 0, 0));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 3, 12, 0, 0));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 0, 0, 0));
            result[3].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 12, 0, 0));
            result[4].NextExecutionDate.Should().Be(new DateTime(2020, 1, 14, 0, 0, 0));
            result[5].NextExecutionDate.Should().Be(new DateTime(2020, 1, 14, 12, 0, 0));
            result[6].NextExecutionDate.Should().Be(new DateTime(2020, 1, 17, 0, 0, 0));
            result[7].NextExecutionDate.Should().Be(new DateTime(2020, 1, 17, 12, 0, 0));
            result[8].NextExecutionDate.Should().Be(new DateTime(2020, 1, 19, 0, 0, 0));
            result[9].NextExecutionDate.Should().Be(new DateTime(2020, 1, 19, 12, 0, 0));

            result[0].Description.Should().Be(@"Occurs every 2 weeks on Tuesday, Friday and Sunday every 12 Hours between 00:00:00 and 23:59:58");
        }

        [Fact]
        public void Calculate_Next_Execution_Recurring_Weekly_Hours()
        {
            Configuration configuration = new()
            {
                Enabled = true,
                Type = ConfigurationTypes.Recurring,
                CurrentDate = new DateTime(2020, 1, 1),
                Periodcity = PeriodicityType.Weekly,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Saturday },
                WeeklyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityType.Hours,
                DailyStartTime = new TimeSpan(4, 0, 0),
                DailyEndTime = new TimeSpan(8, 0, 0),
                StartDate = new DateTime(2020, 1, 2),
                EndDate = new DateTime(2020, 1, 31)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(10);

            result.Length.Should().Be(10);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 4, 4, 0, 0));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 4, 6, 0, 0));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 4, 8, 0, 0));
            result[3].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 0, 0));
            result[4].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 6, 0, 0));
            result[5].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 8, 0, 0));
            result[6].NextExecutionDate.Should().Be(new DateTime(2020, 1, 18, 4, 0, 0));
            result[7].NextExecutionDate.Should().Be(new DateTime(2020, 1, 18, 6, 0, 0));
            result[8].NextExecutionDate.Should().Be(new DateTime(2020, 1, 18, 8, 0, 0));
            result[9].NextExecutionDate.Should().Be(new DateTime(2020, 1, 27, 4, 0, 0));
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
                Periodcity = PeriodicityType.Weekly,
                WeeklyActiveDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday },
                WeeklyPeriodicity = 2,
                DailyType = ConfigurationTypes.Recurring,
                DailyPeriodicity = 2,
                DailyPeriodicityType = TimePeriodicityType.Minutes,
                DailyStartTime = new TimeSpan(4, 30, 0),
                DailyEndTime = new TimeSpan(4, 35, 0),
                StartDate = new DateTime(2020, 1, 2)
            };
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 6, 4, 30, 0));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 6, 4, 32, 0));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 6, 4, 34, 0));
            result[3].NextExecutionDate.Should().Be(new DateTime(2020, 1, 8, 4, 30, 0));
            result[4].NextExecutionDate.Should().Be(new DateTime(2020, 1, 8, 4, 32, 0));
            result[5].NextExecutionDate.Should().Be(new DateTime(2020, 1, 8, 4, 34, 0));
            result[6].NextExecutionDate.Should().Be(new DateTime(2020, 1, 20, 4, 30, 0));
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
            Schedule Schedule = new(configuration);
            var result = Schedule.CalculateSerie(7);

            result.Length.Should().Be(7);
            result[0].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 4, 30, 10));
            result[1].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 4, 30, 12));
            result[2].NextExecutionDate.Should().Be(new DateTime(2020, 1, 5, 4, 30, 14));
            result[3].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 30, 10));
            result[4].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 30, 12));
            result[5].NextExecutionDate.Should().Be(new DateTime(2020, 1, 13, 4, 30, 14));
            result[6].NextExecutionDate.Should().Be(new DateTime(2020, 1, 19, 4, 30, 10));

            result[0].Description.Should().Be(@"Occurs every 2 weeks on Monday and Sunday every 2 Seconds between 04:30:10 and 04:30:15 starting on 02/01/2020");
        }

        #endregion
    }
}