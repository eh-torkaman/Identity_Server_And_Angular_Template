using AutoMapper;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.ModelBinders;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Model;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using Microsoft.AspNetCore.Authorization;

namespace CompanyEmployees.Controllers
{
    [Route("api/Companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ILoggerManager<CompaniesController> logger;
        private readonly IRepositoryManager repository;
        private readonly IMapper mapper;

        public CompaniesController(ILoggerManager<CompaniesController> logger, IRepositoryManager reposetoryManager, IMapper mapper)
        {
            this.logger = logger;
            this.repository = reposetoryManager;
            this.mapper = mapper;
        }
        [HttpGet(Name = nameof(GetAllCompanies))]
        //  [ResponseCache(Duration = 60,VaryByQueryKeys =new [] { "*"})]
        public ActionResult<IEnumerable<CompanyDto>> GetAllCompanies([FromQuery] CompanyParameters companyParameters)
        {
            PagedList<Company> allCompanies = repository.Company.GetAllCompanies(companyParameters, trackChanges: false);

            var lsCompanyDtos = mapper.Map<IEnumerable<CompanyDto>>(allCompanies);

            //todo be result filter montaghel shavad?
            HttpContext.Response.Headers.Add("X-Pagination", allCompanies.XPaginationStr);

            return Ok(lsCompanyDtos);

        }

        [HttpGet("{id:Guid}", Name = nameof(GetCompany))]
        public ActionResult<CompanyDto> GetCompany(Guid id)
        {
            logger.LogWarn("Hi body");
            throw new Exception("sdfsdfsdfs");
            var company = repository.Company.GetCompany(id, trackChanges: false);
            if (company == null)
                return NotFound();
            return mapper.Map<CompanyDto>(company);
        }

        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost(Name = nameof(CreateCompany))]
        public ActionResult<CompanyDto> CreateCompany([FromBody] CompanyForCreationDto companyForCreationDto)
        {
            var company = mapper.Map<Company>(companyForCreationDto);
            company.Id = Guid.NewGuid();
            repository.Company.CreateCompany(company);
            repository.Save();
            return CreatedAtRoute(nameof(GetCompany), new { id = company.Id }, mapper.Map<CompanyDto>(company));
        }


        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public ActionResult<IEnumerable<CompanyDto>>
            GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companyEntities = repository.Company.GetByIds(ids, trackChanges: false);
            if (ids.Count() != companyEntities.Count())
            {
                logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var companiesToReturn = mapper.Map<List<CompanyDto>>(companyEntities);
            return companiesToReturn;
        }

        [HttpPost("collection")]
        public ActionResult<IEnumerable<CompanyDto>> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }
            var companyEntities = mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                repository.Company.CreateCompany(company);
            }
            repository.Save();
            var companyCollectionToReturn =
            mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCompany(Guid id)
        {
            var company = repository.Company.GetCompany(id, trackChanges: false);
            if (company == null)
            {
                logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            repository.Company.DeleteCompany(company);
            repository.Save();
            return NoContent();
        }



        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute), Order = 10)]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute), Order = 100)]
        public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            //if (company == null)
            //{
            //    logger.LogError("CompanyForUpdateDto object sent from client is null.");
            //    return BadRequest("CompanyForUpdateDto object is null");
            //}
            var companyEntity = HttpContext.Items["company"] as Company; //await repository.Company.GetCompany(id, trackChanges: true);
            //if (companyEntity == null)
            //{
            //    logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
            //    return NotFound();
            //}
            mapper.Map(company, companyEntity);
            repository.Save();
            return NoContent();
        }
    }
}
