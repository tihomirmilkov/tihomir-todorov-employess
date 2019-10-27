using System;

namespace tihomir_todorov_employess.Models
{
    public class EmployeesCoupleModel
    {
        public int EmployeeID1 { get; set; }
        public int EmployeeID2 { get; set; }
        public int ProjectID { get; set; }
        public int DaysWorked { get; set; }

        public EmployeesCoupleModel()
        {

        }
    }
}
