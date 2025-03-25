using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OuttputShiftCSV
{
    internal class ShiftPattern
    {
        private string patternCode;
        public string PatternCode
        {
            get { return patternCode; }
        }
        private Shift shift;
        public DateTime StartDateTime
        {
            get{ return shift.StartDateTime; }
        }
        public DateTime EndDateTime
        {
            get { return shift.EndDateTime; }
        }

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
        public ShiftPattern(string patternCode, Shift shift)
        {
            this.patternCode = patternCode;
            this.shift = shift;
        }

        public override string ToString()
        {
            return "パターンコード: " + this.patternCode + 
                " " + "開始:" + shift.StartDateTime.ToString("HH:mm") +
                " " + "終了:" + shift.EndDateTime.ToString("HH:mm");
        }

    }
}
