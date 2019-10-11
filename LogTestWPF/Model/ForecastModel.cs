using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTestWPF.Model
{
    public class ForecastModel
    {
        public DataModel CurrentDataModel { get; set; }
        public List<DataModel> DataModels { get; set; }
        public List<ProbabilityModel> ProbabilityModels { get; set; }
        public double CurrentTime { get; set; }
        public int CurrentScoreFirstCommand { get; set; }
        public int CurrentScoreSecondCommand { get; set; }
    }
}
