using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuttputShiftCSV
{
    internal class Employee
    {
        private int employeeId;
        private string name;
        private List<Shift> shiftList;

        public Employee()
        {
            throw new NotImplementedException();
        }

        public Employee(int employeeId,string name)
        {
            this.employeeId = employeeId;
            this.name = name;
            this.shiftList = new List<Shift>();
        }

        public List<Shift> GetShiftList()
        {
            return new List<Shift>(this.shiftList);
        }

        public void AddShift(Shift shift)
        {
            this.shiftList.Add(shift);
        }
    }
}
