using tihomir_todorov_employess.Utilities;

namespace tihomir_todorov_employess.Models
{
    public class ResultModel
    {
        public EmployeesCouple employeesCouple { get; set; }

        // TODO make enum with error messages
        public string ErrorMessage { get; set; }
    }
}
