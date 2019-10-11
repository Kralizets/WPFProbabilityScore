using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTestWPF.Model
{
    public enum ResultCommandEnum
    {
        FirstCommandWin = 0,
        SecondCommandWin = 1,
        Draw = 2,

        #region extenstion result
        //каждая команда забьет хотя бы по одному мячу
        OneScoreFirstAndSecondCommand = 3,

        //первая команда забьет хотя бы один гол
        FirstCommandScoreOne = 4,

        //вторая команда забьет хотя бы один гол
        SecondCommandScoreOne = 5,

        //в матче будет забито хотя бы два гола
        TwoScoreInAllCommand = 6
        #endregion
    }
}
