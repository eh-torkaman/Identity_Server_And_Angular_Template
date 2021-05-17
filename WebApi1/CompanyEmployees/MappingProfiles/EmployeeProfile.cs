using AutoMapper;
using Entities.Model;
using Entities.DataTransferObjects;
namespace CompanyEmployees.MappingProfiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeForCreationDto, Employee>();
            CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap(); ;
        }
    }
}
