using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semicrol.Schedule
{
    public class OutPut
    {
        public DateTime NextExecutionDate { get; set; }
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            return ((OutPut)obj).NextExecutionDate == this.NextExecutionDate &&
                   ((OutPut)obj).Description == this.Description;                   
        }
    }
}
