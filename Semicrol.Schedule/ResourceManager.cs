using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class ResourceManager
    {
        private readonly CultureInfo _culture;
        Dictionary<string, string> _resources;

        public ResourceManager(SupportedCultures language)
        {
            _culture = new CultureInfo(language.ToString());
            _resources = new Dictionary<string, string>();
            this.LoadResources();
        }

        private void LoadResources()
        {
            if (_culture.Name == "es-ES")
            {                
                _resources.Add("disabled", "El proceso está desactivado");
                _resources.Add("configurationUndefined", "Debe definir una configuración para el horario");

                #region ValidatorClass
                _resources.Add("currentCorrect", "La fecha actual debe ser una fecha correcta");
                _resources.Add("startCorrect", "La fecha de inicio debe ser una fecha correcta");
                _resources.Add("endCorrect", "La fecha de fin debe ser una fecha correcta");
                _resources.Add("endGreater", "La fecha de fin debe ser mayor que la de inicio");
                _resources.Add("onceTypeCorrect", "Si el tipo es 'una vez', debe introducir una fecha actual válida");
                _resources.Add("weeklyperiodicityValidation", "La periodicidad semanal debe ser un número correcto y mayor que 0 si la configuración es semanal");
                _resources.Add("daySelectedWeek", "Debe seleccionar algún día de la semana si la configuración es semanal");
                _resources.Add("periodicityValidation", "Debe indicar una periodicidad correcta");
                _resources.Add("startFrecuency", "La frecuencia diaria de inicio debe ser una hora correcta");
                _resources.Add("endFrecuency", "La frecuencia diaria final debe ser una hora correcta distinta de cero");
                _resources.Add("intervalTime", "El tiempo de intervalo en la frecuencia diaria debe ser inferior a 24 horas");
                _resources.Add("nextExecution", "La siguiente hora de ejecución no puede ser inferior a la fecha actual");
                _resources.Add("limitsValidation", "La fecha está fuera de los límites");
                _resources.Add("monthlyPeriodicity", "Debe introducir una periodicidad mensual válida");
                _resources.Add("validDay", "Debe introducir un día válido");
                #endregion

                #region DescriptionClass
                _resources.Add("occurs", "Se produce ");
                _resources.Add("everyday", "todos los días");
                _resources.Add("once", "una vez");
                _resources.Add("at", "a las");
                _resources.Add("usedon", ". El calendario se utilizará el");
                _resources.Add("every", "cada");
                _resources.Add("between", "entre");
                _resources.Add("and", "y");
                _resources.Add("week", "semana");
                _resources.Add("weeks", "semanas");
                _resources.Add("on", "los");
                _resources.Add(",", ", ");
                _resources.Add("thedays", "los días");
                _resources.Add("months", "meses");
                _resources.Add("the", "el");
                _resources.Add("ofevery", "de cada");
                _resources.Add("startingon", "empezando el");
                _resources.Add("endingon", "terminando el");
                #endregion

                #region OrdinalPeriodicityTypes
                _resources.Add("first", "primer");
                _resources.Add("second", "segundo");
                _resources.Add("third", "tercer");
                _resources.Add("fourth", "cuarto");
                _resources.Add("last", "último");
                #endregion

                #region AvailableWeekDays
                _resources.Add("monday", "lunes");
                _resources.Add("tuesday", "martes");
                _resources.Add("wednesday", "miércoles");
                _resources.Add("thursday", "jueves");
                _resources.Add("friday", "viernes");
                _resources.Add("saturday", "sábado");
                _resources.Add("sunday", "domingo");
                _resources.Add("day", "día");
                _resources.Add("weekDay", "día de la semana");
                _resources.Add("weekendDay", "fin de semana");
                #endregion

                #region TimePeriodicityTypes
                _resources.Add("hours", "horas");
                _resources.Add("minutes", "minutos");
                _resources.Add("seconds", "segundos");
                #endregion

                return;
            }
                        
            _resources.Add("disabled", "The process is disabled");
            _resources.Add("configurationUndefined", "You should define a configuration for the schedule");

            #region ValidatorClass
            _resources.Add("currentCorrect", "Current date should be a correct date");
            _resources.Add("startCorrect", "Start Date should be a correct date");
            _resources.Add("endCorrect", "End Date should be a correct date");
            _resources.Add("endGreater", "End date should be greater than Start date");
            _resources.Add("onceTypeCorrect", "If type is Once, you should enter a valid DateTime");
            _resources.Add("weeklyperiodicityValidation", "Weekly periodicity should be a correct number and greater than 0 if configuration occurs weekly");
            _resources.Add("daySelectedWeek", "You should select some day of the week if configuration occurs weekly");
            _resources.Add("periodicityValidation", "You should indicate a correct periodicity");
            _resources.Add("startFrecuency", "Start Daily Frecuency should be a correct time");
            _resources.Add("endFrecuency", "End Daily Frecuency should be a correct time distinct of zero");
            _resources.Add("intervalTime", "The interval time in daily frecuency should be lower than 24 hours");
            _resources.Add("nextExecution", "Next execution time could not be lower than Current date");
            _resources.Add("limitsValidation", "The date is out of the limits");
            _resources.Add("monthlyPeriodicity", "You should enter a valid monthly periodicity");
            _resources.Add("validDay", "You should enter a valid day");
            #endregion

            #region DescriptionClass
            _resources.Add("occurs", "Occurs ");
            _resources.Add("everyday", "every day");
            _resources.Add("once", "once");
            _resources.Add("at","at");
            _resources.Add("usedon", ". Schedule will be used on");
            _resources.Add("every", "every");
            _resources.Add("between", "between");
            _resources.Add("and", "and");
            _resources.Add("week", "week");
            _resources.Add("weeks", "weeks");
            _resources.Add("on", "on");
            _resources.Add(",", ", ");
            _resources.Add("thedays", "the days");
            _resources.Add("months", "months");
            _resources.Add("the", "the");
            _resources.Add("ofevery", "of every");
            _resources.Add("startingon", "starting on");
            _resources.Add("endingon", "ending on");
            #endregion

            #region OrdinalPeriodicityTypes
            _resources.Add("first", "First");
            _resources.Add("second", "Second");
            _resources.Add("third", "Third");
            _resources.Add("fourth", "Fourth");
            _resources.Add("last", "Last");
            #endregion

            #region AvailableWeekDays
            _resources.Add("monday", "Monday");
            _resources.Add("tuesday", "Tuesday");
            _resources.Add("wednesday", "Wednesday");
            _resources.Add("thursday", "Thursday");
            _resources.Add("friday", "Friday");
            _resources.Add("saturday", "Saturday");
            _resources.Add("sunday", "Sunday");
            _resources.Add("day", "Day");
            _resources.Add("weekDay", "WeekDay");
            _resources.Add("weekendDay", "WeekendDay");
            #endregion

            #region TimePeriodicityTypes
            _resources.Add("hours", "Hours");
            _resources.Add("minutes", "Minutes");
            _resources.Add("seconds", "Seconds");
            #endregion
        }

        public string GetResource(string resourceKey)
        {
            bool hasResource = _resources.TryGetValue(resourceKey, out string value);
            return hasResource ? value : "**" + resourceKey + "**";
        }

        public string GetOrdinalPeriodicityTranslated(OrdinalPeriodicityTypes periodicity)
        {
            switch (periodicity)
            {
                case OrdinalPeriodicityTypes.First:
                    return GetResource("first");
                case OrdinalPeriodicityTypes.Second:
                    return GetResource("second");
                case OrdinalPeriodicityTypes.Third:
                    return GetResource("third");
                case OrdinalPeriodicityTypes.Fourth:
                    return GetResource("fourth");
                case OrdinalPeriodicityTypes.Last:
                    return GetResource("last");
                default:
                    return string.Empty;
            }
        }

        public string GetWeekDaysTranslated(AvailableWeekDays weekDay)
        {
            switch (weekDay)
            {
                case AvailableWeekDays.Monday:
                    return GetResource("monday");
                case AvailableWeekDays.Tuesday:
                    return GetResource("tuesday");
                case AvailableWeekDays.Wednesday:
                    return GetResource("wednesday");
                case AvailableWeekDays.Thursday:
                    return GetResource("thursday");
                case AvailableWeekDays.Friday:
                    return GetResource("friday");
                case AvailableWeekDays.Saturday:
                    return GetResource("saturday");
                case AvailableWeekDays.Sunday:
                    return GetResource("sunday");
                case AvailableWeekDays.Day:
                    return GetResource("day");
                case AvailableWeekDays.WeekDay:
                    return GetResource("weekDay");
                case AvailableWeekDays.WeekendDay:
                    return GetResource("weekendDay");
                default:
                    return string.Empty;
            }            
        }

        public string GetWeekDaysTranslated(DayOfWeek weekDay)
        {
            switch (weekDay)
            {
                case DayOfWeek.Monday:
                    return GetResource("monday");
                case DayOfWeek.Tuesday:
                    return GetResource("tuesday");
                case DayOfWeek.Wednesday:
                    return GetResource("wednesday");
                case DayOfWeek.Thursday:
                    return GetResource("thursday");
                case DayOfWeek.Friday:
                    return GetResource("friday");
                case DayOfWeek.Saturday:
                    return GetResource("saturday");
                case DayOfWeek.Sunday:
                    return GetResource("sunday");
                default:
                    return string.Empty;
            }
        }

        public string GetTimePeriodicityTranslated(TimePeriodicityTypes periodicity)
        {
            switch (periodicity)
            {
                case TimePeriodicityTypes.Hours:
                    return GetResource("hours");
                case TimePeriodicityTypes.Minutes:
                    return GetResource("minutes");
                case TimePeriodicityTypes.Seconds:
                    return GetResource("seconds");
                default:
                    return string.Empty;
            }
        }

        public string GetFormattedDate(DateTime date)
        {
            return date.ToString("d", _culture);
        }

        public string GetFormattedTime(TimeSpan time)
        {
            return time.ToString("t", _culture);
        }

        public string GetFormattedTime(DateTime date)
        {
            return date.ToString("t", _culture);
        }
    }
}
