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

        public void AddShift(ShiftPattern shift)
        {
            this.shiftList.Add(shift);
        }
    }
}
