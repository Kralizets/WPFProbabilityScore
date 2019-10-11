using LogTestWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTestWPF.Logic
{
    public static class ProbabilityProvider
    {
        public static ResultCommandEnum GetResultCommand(int scoreFirstCommand, int scoreSecondCommand)
        {
            if (scoreFirstCommand > scoreSecondCommand)
            {
                return ResultCommandEnum.FirstCommandWin;
            }

            if (scoreSecondCommand > scoreFirstCommand)
            {
                return ResultCommandEnum.SecondCommandWin;
            }

            return ResultCommandEnum.Draw;
        }
        public static List<ProbabilityModel> GetProbabilityModels(List<DataModel> data)
        {
            return new List<ProbabilityModel>
            {
                new ProbabilityModel
                {
                    Result = ResultCommandEnum.FirstCommandWin,
                    Probability = data
                        .Where(x => x.ResultCommand == ResultCommandEnum.FirstCommandWin)
                        .Select(x => x.Probability)
                        .Sum()
                },
                new ProbabilityModel
                {
                    Result = ResultCommandEnum.SecondCommandWin,
                    Probability = data
                        .Where(x => x.ResultCommand == ResultCommandEnum.SecondCommandWin)
                        .Select(x => x.Probability)
                        .Sum()
                },
                new ProbabilityModel
                {
                    Result = ResultCommandEnum.Draw,
                    Probability = data
                        .Where(x => x.ResultCommand == ResultCommandEnum.Draw)
                        .Select(x => x.Probability)
                        .Sum()
                }
            };
        }

        public static List<DataModel> GetCurrentDataModels(List<DataModel> initialData, double timeLeftKoef,
            int scoreFirstCommand, int scoreSecondCommand)
        {
            List<DataModel> resultData = new List<DataModel>();

            foreach (var item in initialData)
            {
                DataModel currentResult = new DataModel();

                currentResult.Probability = item.Probability * timeLeftKoef;
                currentResult.Score = $"{item.ScoreFirstCommand + scoreFirstCommand}-{item.ScoreSecondCommand + scoreSecondCommand}";
                currentResult.ScoreFirstCommand = item.ScoreFirstCommand;
                currentResult.ScoreSecondCommand = item.ScoreSecondCommand;
                currentResult.ResultCommand = GetResultCommand(item.ScoreFirstCommand, item.ScoreSecondCommand);

                resultData.Add(currentResult);
            }

            return resultData;
        }

        public static DataModel GetCurrentDataModel(int scoreFirstCommand, int scoreSecondCommand, double probability)
        {
            return new DataModel
            {
                Score = $"{scoreFirstCommand}-{scoreSecondCommand}",
                Probability = probability,
                ScoreFirstCommand = scoreFirstCommand,
                ScoreSecondCommand = scoreSecondCommand,
                ResultCommand = GetResultCommand(scoreFirstCommand, scoreSecondCommand)
            };
        }

        public static ProbabilityModel GetProbilityCommandWinOrDrawOrLose(ResultCommandEnum result, DataModel currentDataModel, List<DataModel> dataModels)
        {
            var possibleOutcome = dataModels
                .Where(x => x.ResultCommand == result)
                .Sum(x => x.Probability);

            return new ProbabilityModel
            {
                Probability = currentDataModel.ResultCommand == result ? (possibleOutcome + currentDataModel.Probability) : possibleOutcome,
                Result = result
            };                
        }
        public static ProbabilityModel GetProbabilityOneScoreFirstAndSecondCommand(List<DataModel> dataModels)
        {
            return new ProbabilityModel
            {
                Probability = dataModels
                    .Where(x => x.ScoreFirstCommand > 0 && x.ScoreSecondCommand > 0)
                    .Sum(x => x.Probability),
                Result = ResultCommandEnum.OneScoreFirstAndSecondCommand
            };
        }

        public static ProbabilityModel GetProbabilityCommandScoreOne(ResultCommandEnum result, List<DataModel> dataModels)
        {
            return new ProbabilityModel
            {
                Probability = dataModels
                    .Where(x => result == ResultCommandEnum.FirstCommandScoreOne ? x.ScoreFirstCommand > 0 : x.ScoreSecondCommand > 0)
                    .Sum(x => x.Probability),
                Result = result
            };
        }

        public static ProbabilityModel GetProbabilityOneScoreFirstOrSecondCommand(List<DataModel> dataModels)
        {
            return new ProbabilityModel
            {
                Probability = dataModels
                    .Where(x => (x.ScoreFirstCommand + x.ScoreSecondCommand) > 1)
                    .Sum(x => x.Probability),
                Result = ResultCommandEnum.TwoScoreInAllCommand
            };
        }
    }
}
