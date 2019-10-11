using LogTestWPF.Model;
using LogTestWPF.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTestWPF.Logic
{
    public static class InitProvider
    {
        public static ForecastModel GetForecastModel()
        {
            var dataModels = FileRepository.GetDataFromFile();

            return new ForecastModel
            {
                CurrentDataModel = dataModels.FirstOrDefault(),
                DataModels = dataModels,
                ProbabilityModels = new List<ProbabilityModel>(),
                CurrentTime = 0,
                CurrentScoreFirstCommand = 0,
                CurrentScoreSecondCommand = 0,
            };
        }

        public static string GetRegexpValidator(List<DataModel> dataModel)
        {
            var regexResult = $"^([{dataModel.FirstOrDefault().ScoreFirstCommand}][-][{dataModel.FirstOrDefault().ScoreSecondCommand}])";

            for (int i = 1; i < dataModel.Count(); i++)
            {
                regexResult += $"|([{dataModel[i].ScoreFirstCommand}][-][{dataModel[i].ScoreSecondCommand}])";
            }

            return (regexResult + "$");
        }
    }
}
