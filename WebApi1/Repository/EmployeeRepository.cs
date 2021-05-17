using Contracts;
using Entities;
using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository :  RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }

        public Employee GetCompanyEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            return FindByCondition((expr => (expr.Id == id) && (expr.CompanyId == companyId)), trackChanges).SingleOrDefault();

        }

        public IEnumerable<Employee> GetCompanyEmployees(Guid companyId, bool trackChanges)
        {
            return FindByCondition((expr => expr.CompanyId == companyId), trackChanges).ToList();
        }

    }
}
