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

        public Employee()
        {
            throw new NotImplementedException();
        }

        public Employee(int employeeId,string name)
        {
            this.employeeId = employeeId;
            this.name = name;
        }
    }
}
