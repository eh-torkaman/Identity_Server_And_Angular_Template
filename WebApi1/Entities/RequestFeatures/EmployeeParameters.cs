using Entities.Model;

namespace Entities.RequestFeatures
{
    public class EmployeeParameters : RequestParameters<Employee>
    {
        protected override string DefaultOrderColumn => "Position";
    }
}
