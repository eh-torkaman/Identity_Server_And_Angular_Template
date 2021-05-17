using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeRepository
    {
      public  IEnumerable<Employee> GetCompanyEmployees(Guid companyId, bool trackChanges);
        public Employee GetCompanyEmployee(Guid companyId, Guid id, bool trackChanges);
        public void CreateEmployeeForCompany(Guid companyId, Employee employee);
        public void DeleteEmployee(Employee employee);
    }
}
