using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OuttputShiftCSV
{
    internal class ShiftPattern
    {
        private string patternCode;
        private DateTime startDate;
        private DateTime endDate;

        public ShiftPattern()
        {
            throw new NotImplementedException();
        }

        public ShiftPattern(string patternCode, string startDateStr, string endDateStr)
        {
            //BASE_DATEの日時にはこだわりはない。明示的な固定値指定にのみ意図がある。
            const string BASE_DATE ="2025/03/23 ";

            this.patternCode = patternCode;
            this.startDate = DateTime.Parse(BASE_DATE + zenToHanComvert(startDateStr));
            this.endDate = DateTime.Parse(BASE_DATE + zenToHanComvert(endDateStr));
        }

        public override string ToString()
        {
            return "パターンコード: " + this.patternCode + 
                " " + "開始:" +startDate.ToString("HH:mm") +
                " " + "終了:" + endDate.ToString("HH:mm");
        }

        private string zenToHanComvert(string dateTimeStr)
        {
            string result;
            //「：」→「:」
            result = dateTimeStr.Replace("：", ":");

            //数字置換
            result = Regex.Replace(result, "[０-９]", p => ((char)(p.Value[0] - '０' + '0')).ToString());
            return result;

        }
            

    }
}
