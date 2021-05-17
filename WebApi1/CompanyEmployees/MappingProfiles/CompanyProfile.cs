using AutoMapper;
using Entities.Model;
using Entities.DataTransferObjects;
namespace CompanyEmployees.MappingProfiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<Company, CompanyDto>().ForMember(c => c.FullAddress
             , opt => opt.MapFrom(cDto => cDto.Country + " " + cDto.Address)
             );

            CreateMap<CompanyForUpdateDto, Company>();
            CreateMap<CompanyForCreationDto, Company>();
        }
    }
}
