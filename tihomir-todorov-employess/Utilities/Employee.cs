using System;

namespace tihomir_todorov_employess.Utilities
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public int ProjectID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public Employee()
        {

        }

        public Employee(int employeeID, int projectID, DateTime dateFrom, DateTime dateTo)
        {
            EmployeeID = employeeID;
            ProjectID = projectID;
            DateFrom = dateFrom;
            DateTo = dateTo;
        }
    }
}
