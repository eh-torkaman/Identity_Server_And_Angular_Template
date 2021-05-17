using Entities.Model;
using System.Reflection;

namespace Entities.RequestFeatures
{
    public class CompanyParameters : RequestParameters<Company>
    {
        protected override string DefaultOrderColumn => typeof(Company).GetProperties(BindingFlags.Public | BindingFlags.Instance)[0].Name;
    }
}
