using Contracts;
using Entities;
using Entities.Model;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Extensions;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        public void CreateCompany(Company company)
        {
            Create(company);
        }

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }

        public PagedList<Company> GetAllCompanies(CompanyParameters param, bool trackChanges)
        {
            var query = FindAll(trackChanges);
            query = param.GetIQueriableByOrderStr(query); ;
            query = param.TextFilter_Strings(query);
            var rs = new PagedList<Company>(query, param.PageSize, param.PageNumber);
            return rs;
        }

        public   IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
        {
            return   FindByCondition(x => ids.Contains(x.Id), trackChanges).ToList();
        }

        public Company GetCompany(Guid id, bool trackChanges)
        {
            return  FindByCondition((t => t.Id == id), trackChanges).SingleOrDefault();
        }


    }
}
