using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTestWPF.Model
{
    public class DataModel
    {
        public string Score { get; set; }
        public int ScoreFirstCommand { get; set; }
        public int ScoreSecondCommand { get; set; }
        public double Probability { get; set; }
        public ResultCommandEnum ResultCommand { get; set; }
    }
}
