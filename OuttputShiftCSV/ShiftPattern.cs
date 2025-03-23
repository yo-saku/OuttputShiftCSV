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
        private DateTime workDate;
        private DateTime startDateTime;
        private DateTime endDateTime;

        public ShiftPattern()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="patternCode">シフトパターンコード</param>
        /// <param name="workDate">作業日(指定が無い場合、「0001/01/01/01」</param>
        /// <param name="startTimeStr">作業開始時刻文字列</param>
        /// <param name="endTimeStr">作業終了時刻文字列</param>
        public ShiftPattern(string patternCode, string startTimeStr, string endTimeStr, DateTime workDate= default(DateTime))
        {
            this.patternCode = patternCode;
            this.workDate = workDate;
            this.startDateTime = DateTime.Parse(workDate.ToString("yyyy/MM/dd ") + zenToHanComvert(startTimeStr));
            this.endDateTime = DateTime.Parse(workDate.ToString("yyyy/MM/dd ") + zenToHanComvert(endTimeStr));
        }

        public override string ToString()
        {
            return "勤務日(" + workDate.ToString("yyyy/MM/dd") +")" +
                " " + "パターンコード: " + this.patternCode + 
                " " + "開始:" +startDateTime.ToString("HH:mm") +
                " " + "終了:" + endDateTime.ToString("HH:mm");
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
