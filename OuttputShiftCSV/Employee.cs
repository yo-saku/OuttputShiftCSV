using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuttputShiftCSV
{
    internal class Employee
    {
        private string employeeId;
        private string name;
        private List<ShiftPattern> shiftList;

        public Employee()
        {
            throw new NotImplementedException();
        }

        public Employee(string employeeId,string name)
        {
            this.employeeId = employeeId;
            this.name = name;
            this.shiftList = new List<ShiftPattern>();
        }

        /// <summary>
        /// シフト情報をCSV書き込み用の文字列にする
        /// </summary>
        /// <returns></returns>
        public string GetShiftCsvData()
        {
            string result = "";
            foreach(ShiftPattern shiftPattern in shiftList)
            {
                if(result != "")
                {
                    result = result + Environment.NewLine;
                }
                result = result + shiftPattern.StartDateTime.ToString("yyyyMMdd") + "," + this.employeeId + "," + shiftPattern.PatternCode;
            }

            return result;
        }

        /// <summary>
        /// シフトパターンを追加する
        /// </summary>
        /// <param name="shift"></param>
        public void AddShift(ShiftPattern shift)
        {
            this.shiftList.Add(shift);
        }
    }
}
