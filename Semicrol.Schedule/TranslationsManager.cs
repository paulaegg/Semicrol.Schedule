using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class TranslationsManager
    {
        private CultureInfo _cultureInfo;
        Dictionary<string, string> _translations = new Dictionary<string, string>();
        public TranslationsManager(SupportingLanguages language)
        {
            _cultureInfo = new CultureInfo(language.ToString());
            LoadResources();
        }

        private void LoadResources()
        {
            if (_cultureInfo.Name == "es-ES")
            {
                _translations.Add("disabled", "The process is disabled");
                _translations.Add("currentCorrect", "La fecha actual debe ser correcta");
                _translations.Add("startDateCorrect", "La fecha de inicio debe ser correcta");
                _translations.Add("endDateCorrect", "La fecha de fin debe ser correcta");
                _translations.Add("endGreater", "La fecha de fin debe ser mayor que la fecha de inicio");
                _translations.Add("onceValidation", "Si el tipo es 'una vez', debe introducir una fecha actual válida");
                _translations.Add("weeklyPeriodicityCorrect", "La periodicidad semanal debe ser un número correcto y mayor que 0 si la configuración es semanal");
                _translations.Add("weekDaysSelected", "Debe seleccionar algún día de la semana si la configuración es semanal");
                return;
            }
            _translations.Add("disabled", "The process is disabled");
            _translations.Add("currentCorrect", "Current date should be a correct date");
            _translations.Add("startDateCorrect", "Start Date should be a correct date");
            _translations.Add("endDateCorrect", "End Date should be a correct date");
            _translations.Add("endGreater", "End date should be greater than Start date");
            _translations.Add("onceValidation", "If type is Once, you should enter a valid DateTime");
            _translations.Add("weeklyPeriodicityCorrect", "Weekly periodicity should be a correct number and greater than 0 if configuration occurs weekly");
            _translations.Add("weekDaysSelected", "You should select some day of the week if configuration occurs weekly");

            //_translations.Add("", );
        }

        public string GetText(string key)
        {
            bool hasValue = _translations.TryGetValue(key, out string value);
            return hasValue ? value : key;            
        }
    }
}
