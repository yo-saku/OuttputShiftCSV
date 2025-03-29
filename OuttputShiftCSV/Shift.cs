using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OuttputShiftCSV
{
    internal class Shift
    {
        private DateTime workDate;
        private DateTime startDateTime;
        public DateTime StartDateTime
        {
            get { return startDateTime; }
            set { startDateTime = value; }
        }
        private DateTime endDateTime;
        public DateTime EndDateTime
        {
            get { return endDateTime; }
            set { endDateTime = value; }
        }

        public Shift(string startTimeStr, string endTimeStr, DateTime workDate = default(DateTime))
        {
            this.workDate = workDate;
            this.startDateTime = DateTime.Parse(workDate.ToString("yyyy/MM/dd ") + ZenToHanComvert(startTimeStr));
            this.endDateTime = DateTime.Parse(workDate.ToString("yyyy/MM/dd ") + ZenToHanComvert(endTimeStr));
        }

        /// <summary>
        /// 全角を半角に変換
        /// </summary>
        /// <param name="dateTimeStr">時刻文字列</param>
        /// <returns></returns>
        private string ZenToHanComvert(string dateTimeStr)
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
