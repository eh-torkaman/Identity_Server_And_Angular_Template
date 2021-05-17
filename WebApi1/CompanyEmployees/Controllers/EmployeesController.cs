using AutoMapper;
using CompanyEmployees.Extensions;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [Route("api/Companies/{companyId:Guid}/Employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ILoggerManager<EmployeesController> logger;
        private readonly IRepositoryManager repository;
        private readonly IMapper mapper;
        public EmployeesController(ILoggerManager<EmployeesController> logger, IRepositoryManager reposetoryManager, IMapper mapper )
        {
            this.logger = logger;
            this.repository = reposetoryManager;
            this.mapper = mapper;
        }
        [HttpGet(Name = nameof(GetCompanyEmployees))]
        public ActionResult<IEnumerable<EmployeeDto>> GetCompanyEmployees(Guid companyId)
        {
            var comp = repository.Company.GetCompany(companyId, trackChanges: false);
            if (comp == null)
                return NotFound();
            var companyEmployees = repository.Employee.GetCompanyEmployees(companyId, trackChanges: false);
            return mapper.Map<List<EmployeeDto>>(companyEmployees);

        }


        [HttpGet("{id:Guid}", Name = nameof(GetCompanyEmployee))]
        public ActionResult<EmployeeDto> GetCompanyEmployee(Guid companyId, Guid id)
        {
            var companyEmployee = repository.Employee.GetCompanyEmployee(companyId, id, trackChanges: false);
            if (companyEmployee == null)
                return NotFound();
            return mapper.Map<EmployeeDto>(companyEmployee);
        }


        [HttpPost]

        public ActionResult<EmployeeDto> CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreationDto)
        {
            if (repository.Company.GetCompany(companyId, false) == null)
                return NotFound();
            var emp = mapper.Map<Employee>(employeeForCreationDto);
            emp.Id = Guid.NewGuid();
            repository.Employee.CreateEmployeeForCompany(companyId, emp);
            repository.Save();
            return CreatedAtRoute(nameof(GetCompanyEmployee), new { companyId, id = emp.Id },
                mapper.Map<EmployeeDto>(emp));
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeForCompany = repository.Employee.GetCompanyEmployee(companyId, id, trackChanges: false);
            if (employeeForCompany == null)
            {
                logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            repository.Employee.DeleteEmployee(employeeForCompany);
            repository.Save();
            return NoContent();
        }


        [HttpPut("{id}")]
        public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            if (employee == null)
            {
                logger.LogError("EmployeeForUpdateDto object sent from client is null.");
                return BadRequest("EmployeeForUpdateDto object is null");
            }
            var company = repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                logger.LogInfo($"Company with id: {companyId} doesn't exist in the                database.");
                return NotFound();
            }
            var employeeEntity = repository.Employee.GetCompanyEmployee(companyId, id, trackChanges: true);
            if (employeeEntity == null)
            {
                logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            mapper.Map(employee, employeeEntity);
            repository.Save();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
            [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }
                if (repository.Company.GetCompany(companyId, false) == null)
                return NotFound();
            var employee= repository.Employee.GetCompanyEmployee(companyId, id, true);
            if (employee==null)
                return NotFound();
            var employeeForUpdateDto= mapper.Map<EmployeeForUpdateDto>(employee);

            //patchDoc.ApplyTo(employeeForUpdateDto,ModelState);
            //if (!TryValidateModel(employeeForUpdateDto))
            //{
            //    return ValidationProblem(ModelState);
            //}


           var thisActionResultWontBeNullOnValidationProblem= patchDoc.ApplyToAndCheckModel(employeeForUpdateDto, this);
           if ( thisActionResultWontBeNullOnValidationProblem!=null)
                return thisActionResultWontBeNullOnValidationProblem;

            mapper.Map(employeeForUpdateDto, employee);
            repository.Save();
            return Ok();
        }
        /*public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var option = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)option.Value.InvalidModelStateResponseFactory(ControllerContext);

        }*/

    }
}
