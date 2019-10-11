using LogTestWPF.Logic;
using LogTestWPF.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTestWPF.Repository
{
    public static class FileRepository
    {
        public static List<DataModel> GetDataFromFile()
        {
            string currentItem;

            string path = @"E:\Data\DataFootball.txt";
            List<DataModel> currentListDataModels = new List<DataModel>();

            Encoding enc = Encoding.GetEncoding(1251);
            using (StreamReader sr = new StreamReader(path, enc))
            {
                while ((currentItem = sr.ReadLine()) != null)
                {
                    currentListDataModels.Add(GetDataModel(currentItem));
                }

                return currentListDataModels;
            }
        }

        private static DataModel GetDataModel(string currentItem)
        {
            string score = currentItem.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[0];

            int scoreFirstCommand = int.Parse(score.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            int scoreSecondCommand = int.Parse(score.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1]);

            return new DataModel
            {
                Score = score,
                ScoreFirstCommand = scoreFirstCommand,
                ScoreSecondCommand = scoreSecondCommand,
                Probability = double.Parse(currentItem.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[1]),
                ResultCommand = ProbabilityProvider.GetResultCommand(scoreFirstCommand, scoreSecondCommand)
            };
        }        
    }
}
