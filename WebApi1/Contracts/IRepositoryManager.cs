using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        public ICompanyRepository Company { get;  }
        public IEmployeeRepository Employee { get;  }
        public void Save();
    }
}
