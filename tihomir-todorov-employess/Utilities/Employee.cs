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

        // override Equals in order .Distinct to work
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Employee e = (Employee)obj;
                return (EmployeeID == e.EmployeeID) && (ProjectID == e.ProjectID) && (DateFrom.Equals(e.DateFrom)) && (DateTo.Equals(e.DateTo));
            }
        }

        // needed when overriding Equals
        public override int GetHashCode()
        {
            return (EmployeeID << 2) ^ ProjectID ^ DateFrom.GetHashCode() ^ DateTo.GetHashCode();
        }

    }
}
