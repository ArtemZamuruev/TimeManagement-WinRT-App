using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace Time_Management_Program
{
    class Actions
    {
        
        public string Title { get; set; }

        public DateTime DateTimeOfStart { get;set; }

        public int SpendedTimeInSeconds { get; set; }
    }
}
