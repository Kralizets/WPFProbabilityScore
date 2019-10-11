using LogTestWPF.Logic;
using LogTestWPF.Model;
using LogTestWPF.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogTestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<DataModel> dataModels = FileRepository.GetDataFromFile();
        
        public MainWindow()
        {
            InitializeComponent();

            //getData
            GetData();

            //GetResult
            submitButton.Click += btn1_Click;
        }

        private void GetData()
        {
            for (int i = 0; i < dataModels.Count(); i++)
            {
                TextBlock rootTextBlock = new TextBlock();

                rootTextBlock.Name = $"textViewData{i}";
                rootTextBlock.FontSize = 16;
                rootTextBlock.Text = $"Score: {dataModels[i].Score}; P: {dataModels[i].Probability.ToString()}";

                stackPanelViewData.Children.Add(rootTextBlock);
            }
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            ForecastModel forecastModel = InitProvider.GetForecastModel();

            #region validation
            Regex timeRegexp = new Regex(@"^(\d{1,4})$");
            Regex scoreRegexp = new Regex(InitProvider.GetRegexpValidator(dataModels));

            //валидации формы нет, отсев данных происходит в момент клика
            double currentTime = 0;
            double maxTime = 90 * 60;            
            string score = "0-0";

            int scoreFirstCommand = 0;
            int ScoreSecondCommand = 0;


            if (timeRegexp.IsMatch(textBoxTime.Text))
            {
                currentTime = double.Parse(textBoxTime.Text);
            }

            if (scoreRegexp.IsMatch(textBoxScore.Text))
            {
                score = textBoxScore.Text;
                scoreFirstCommand = int.Parse(score.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                ScoreSecondCommand = int.Parse(score.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1]);
            }            

            if (currentTime < 0)
            {
                currentTime = 0;
            }

            if (currentTime > maxTime)
            {
                currentTime = maxTime;
            }            
            #endregion

            //Основная идея - последующий отрезок матча воспринимается как отдельный матч.
            //Чем ближе к концу - тем больше вероятность текущего счета стремится к единице.
            //В момент, когда время равняется 90 минутам, невозможен иной исход, нежели тот, который задан.
            //Данная концепция согласуется с ситуацией, когда на N-ой минуте уже достигнут максимальный по таблице счет.
            //В этом случае, если не воспринимать оставшуюся часть игры, как отдельно взятый матч с ограниченным временем,
            //Можно прийти к результату, что изменений в матче происходить не будет. Однако, это не совсем логично.
            //Поэтому, в зависимости от счета, меняется и таблица результата,
            //И появляется, пусть и с меньшей вероятностью, ситуация, когда будет забито больше мячей, чем за все предбыдущие матчи.

            double timeLeftKoef = (maxTime - currentTime) / maxTime;

            #region update forecastModel          
            var currentDataModels = ProbabilityProvider.GetCurrentDataModels(dataModels, timeLeftKoef,
                scoreFirstCommand, ScoreSecondCommand);
            //Эта вероятность (текущий счет) будет стремиться к единицы к концу матча.
            var currentFreeProbability = 1.0 - currentDataModels.Sum(x => x.Probability);
            var currentDataModel = ProbabilityProvider.GetCurrentDataModel(scoreFirstCommand, ScoreSecondCommand, currentFreeProbability);            

            forecastModel.CurrentDataModel = currentDataModel;
            forecastModel.CurrentScoreFirstCommand = scoreFirstCommand;
            forecastModel.CurrentScoreSecondCommand = ScoreSecondCommand;
            forecastModel.CurrentTime = currentTime;
            forecastModel.DataModels = currentDataModels;

            #region probability
            //Надо ли загонять вероятности в 1? (с позиции математики - надо,
            //Однако, тут вычисления приближенные, и можно их все округлить вниз до знака, и потом добавить к какой-либо
            //(Мне, наверное, пришлась бы по душе большая) вероятности это значение)). Можно и поделить значения
            //Между всеми вероятностями. Но не придется ли снова нормировать значения?
            forecastModel.ProbabilityModels.Add(ProbabilityProvider.GetProbilityCommandWinOrDrawOrLose(
                ResultCommandEnum.FirstCommandWin, currentDataModel, currentDataModels));
            forecastModel.ProbabilityModels.Add(ProbabilityProvider.GetProbilityCommandWinOrDrawOrLose(
                ResultCommandEnum.SecondCommandWin, currentDataModel, currentDataModels));
            forecastModel.ProbabilityModels.Add(ProbabilityProvider.GetProbilityCommandWinOrDrawOrLose(
                ResultCommandEnum.Draw, currentDataModel, currentDataModels));

            forecastModel.ProbabilityModels.Add(ProbabilityProvider.GetProbabilityOneScoreFirstAndSecondCommand(currentDataModels));
            forecastModel.ProbabilityModels.Add(ProbabilityProvider.GetProbabilityCommandScoreOne(
                ResultCommandEnum.FirstCommandScoreOne, currentDataModels));
            forecastModel.ProbabilityModels.Add(ProbabilityProvider.GetProbabilityCommandScoreOne(
                ResultCommandEnum.SecondCommandScoreOne, currentDataModels));
            forecastModel.ProbabilityModels.Add(ProbabilityProvider.GetProbabilityOneScoreFirstOrSecondCommand(currentDataModels));
            #endregion
            #endregion

            GetCurrentProbabilityTable(forecastModel);
            GetCurrentSpecificProbability(forecastModel);
        }

        private void GetCurrentProbabilityTable(ForecastModel forecastModel)
        {
            for (int i = 0; i < forecastModel.DataModels.Count(); i++)
            {
                TextBlock rootTextBlock = new TextBlock();

                rootTextBlock.Name = $"textViewCurrentData{i}";
                rootTextBlock.FontSize = 16;
                rootTextBlock.Text = $"Score: {forecastModel.DataModels[i].Score}; P: {forecastModel.DataModels[i].Probability.ToString()}";

                stackPanelProbabilityAssessment.Children.Add(rootTextBlock);
            }
        }

        private void GetCurrentSpecificProbability(ForecastModel forecastModel)
        {
            #region FirstCommandWin
            TextBlock rootTextBlockFirstCommandWin = new TextBlock();

            rootTextBlockFirstCommandWin.Name = $"textViewFirstCommandWin";
            rootTextBlockFirstCommandWin.FontSize = 16;
            rootTextBlockFirstCommandWin.Text = $"First command win: " +
                $"{forecastModel.ProbabilityModels.SingleOrDefault(x => x.Result == ResultCommandEnum.FirstCommandWin).Probability}";

            stackPanelSettings.Children.Add(rootTextBlockFirstCommandWin);
            #endregion

            #region SecondCommandWin
            TextBlock rootTextBlockSecondCommandWin = new TextBlock();

            rootTextBlockSecondCommandWin.Name = $"textViewSecondCommandWin";
            rootTextBlockSecondCommandWin.FontSize = 16;
            rootTextBlockSecondCommandWin.Text = $"Second command win: " +
                $"{forecastModel.ProbabilityModels.SingleOrDefault(x => x.Result == ResultCommandEnum.SecondCommandWin).Probability}";

            stackPanelSettings.Children.Add(rootTextBlockSecondCommandWin);
            #endregion

            #region Draw
            TextBlock rootTextBlockDraw = new TextBlock();

            rootTextBlockDraw.Name = $"textViewDraw";
            rootTextBlockDraw.FontSize = 16;
            rootTextBlockDraw.Text = $"Draw command game: " +
                $"{forecastModel.ProbabilityModels.SingleOrDefault(x => x.Result == ResultCommandEnum.Draw).Probability}";

            stackPanelSettings.Children.Add(rootTextBlockDraw);
            #endregion

            #region OneScoreFirstAndSecondCommand
            TextBlock rootTextBlockOneScoreFirstAndSecondCommand = new TextBlock();

            rootTextBlockOneScoreFirstAndSecondCommand.Name = $"textViewOneScoreFirstAndSecondCommand";
            rootTextBlockOneScoreFirstAndSecondCommand.FontSize = 16;
            rootTextBlockOneScoreFirstAndSecondCommand.Text = $"Total 1 and 2 above 0: " +
                $"{forecastModel.ProbabilityModels.SingleOrDefault(x => x.Result == ResultCommandEnum.OneScoreFirstAndSecondCommand).Probability}";

            stackPanelSettings.Children.Add(rootTextBlockOneScoreFirstAndSecondCommand);
            #endregion

            #region FirstCommandScoreOne
            TextBlock rootTextBlockFirstCommandScoreOne = new TextBlock();

            rootTextBlockFirstCommandScoreOne.Name = $"textViewFirstCommandScoreOne";
            rootTextBlockFirstCommandScoreOne.FontSize = 16;
            rootTextBlockFirstCommandScoreOne.Text = $"Total 1 above 0: " +
                $"{forecastModel.ProbabilityModels.SingleOrDefault(x => x.Result == ResultCommandEnum.FirstCommandScoreOne).Probability}";

            stackPanelSettings.Children.Add(rootTextBlockFirstCommandScoreOne);
            #endregion

            #region SecondCommandScoreOne
            TextBlock rootTextBlockSecondCommandScoreOne = new TextBlock();

            rootTextBlockSecondCommandScoreOne.Name = $"textViewSecondCommandScoreOne";
            rootTextBlockSecondCommandScoreOne.FontSize = 16;
            rootTextBlockSecondCommandScoreOne.Text = $"Total 2 above 0: " +
                $"{forecastModel.ProbabilityModels.SingleOrDefault(x => x.Result == ResultCommandEnum.SecondCommandScoreOne).Probability}";

            stackPanelSettings.Children.Add(rootTextBlockSecondCommandScoreOne);
            #endregion

            #region TwoScoreInAllCommand
            TextBlock rootTextBlockTwoScoreInAllCommand = new TextBlock();

            rootTextBlockTwoScoreInAllCommand.Name = $"textViewTwoScoreInAllCommand";
            rootTextBlockTwoScoreInAllCommand.FontSize = 16;
            rootTextBlockTwoScoreInAllCommand.Text = $"Total above 1: " +
                $"{forecastModel.ProbabilityModels.SingleOrDefault(x => x.Result == ResultCommandEnum.TwoScoreInAllCommand).Probability}";

            stackPanelSettings.Children.Add(rootTextBlockTwoScoreInAllCommand);
            #endregion
        }
    }
}
