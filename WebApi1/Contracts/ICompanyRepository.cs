using Entities.Model;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyRepository
    {
        public PagedList<Company> GetAllCompanies(CompanyParameters param, bool trackChanges);

        public Company GetCompany(Guid id,bool trackChanges);

        public void CreateCompany(Company company);
        public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
        public void DeleteCompany(Company company);

    }
}
