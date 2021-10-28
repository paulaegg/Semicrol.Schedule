using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semicrol.Schedule
{
    public class Enumerations
    {
        public enum ConfigurationTypes
        {
            Once = 0,
            Recurring = 1
        }

        public enum PeriodicityType
        {
            Daily = 0,
            Weekly = 1
        }

        public enum TimePeriodicityType
        {
            Hours = 0,
            Minutes = 1,
            Seconds = 3
        }
    }
}
