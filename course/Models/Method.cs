using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace course.Models
{
    public class Method
    {
        public double Efficiency { get; set; }
        public int OptionsCount { get; set; }
        public int MonthCount { get; set; }
        public int Result { get; set; }

        public List< List<double> > Data { get; set; }
        public List<double> Efficiencies { get; set; }
    }
}
