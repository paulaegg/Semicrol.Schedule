using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Semicrol.Schedule.Enumerations;

namespace Semicrol.Schedule
{
    public class Configuration
    {
        public Configuration()
        { }

        public DateTime CurrentDate { get; set; }
        public ConfigurationTypes ConfigurationType { get; set; }
        public bool Enabled { get; set; }
        public DateTime? ConfigurationDate { get; set; }
        public int ConfigurationDays { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public TimeSpan PeriodOccurs
        {
            get
            {
                return new TimeSpan(this.ConfigurationDays, 0, 0, 0);
            }
        }
    }
}