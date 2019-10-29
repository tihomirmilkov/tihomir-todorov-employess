using System.Collections.Generic;
using tihomir_todorov_employess.Utilities;

namespace tihomir_todorov_employess.Models
{
    public class ResultModel
    {
        public List<EmployeesSameProjectWorkCouple> employeesCouples { get; set; }

        // TODO make enum with error messages
        public string ErrorMessage { get; set; }
    }
}
