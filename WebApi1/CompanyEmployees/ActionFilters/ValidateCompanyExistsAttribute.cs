using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.ActionFilters
{
    public class ValidateCompanyExistsAttribute : IAsyncActionFilter
    {
        private readonly ILoggerManager logger;
        private readonly IRepositoryManager repository;

        public ValidateCompanyExistsAttribute(ILoggerManager logger,IRepositoryManager repositoryManager )
        {
            this.logger = logger;
           this.repository = repositoryManager;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var trackChanges = context.HttpContext.Request.Method.Equals("PUT");
            var companyId=(Guid)context.ActionArguments.SingleOrDefault(t => t.Value is Guid).Value;
            var company =   repository.Company.GetCompany(companyId, trackChanges);
            if (company == null)
            {
                logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("company", company);
            }
                var actionExecutedContext = await next(); 

        }
    }
}
