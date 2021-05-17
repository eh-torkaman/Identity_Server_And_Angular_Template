using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using CompanyEmployees.Extensions;
using System.IO;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using CompanyEmployees.InputOutputFormatter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using CompanyEmployees.ActionFilters;
using Entities.DataTransferObjects;
using AspNetCoreRateLimit;
using Entities.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace CompanyEmployees
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                //.RequireRole( "WebApi1Users")//.RequireRole(new []{ "WebApi1",""})
                                .Build();
            //services.Configure<ApiBehaviorOptions>(confOpt => {confOpt.SuppressModelStateInvalidFilter = true;            });

            services.AddControllers(configureMvcOption =>
            {

                configureMvcOption.RespectBrowserAcceptHeader = true;
                configureMvcOption.ReturnHttpNotAcceptable = true;
                configureMvcOption.Filters.Add(new AuthorizeFilter(policy));
             //   configureMvcOption.CacheProfiles.Add("profile001", new CacheProfile() { Duration = 20, VaryByQueryKeys = new[] { "*" } });
                // configureMvcOption.OutputFormatters.Add(new CsvOutputFormatter());
            }
            ).AddNewtonsoftJson()
            .AddCustomCSVFormatter()
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var problemDetailsFactory = actionContext.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(actionContext.HttpContext, actionContext.ModelState);
                    problemDetails.Detail = "see the errors field for detail ";
                    problemDetails.Instance = actionContext.HttpContext.Request.Path;
                    problemDetails.Extensions.TryAdd("traceId", actionContext.HttpContext.TraceIdentifier);
                    problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                    problemDetails.Title = "one or more validation errors occurred";
                    return new UnprocessableEntityObjectResult(problemDetails) { ContentTypes = { "application/problem+json" } };

                };
            });



            #region ServiceExtentions
            ////////////////////////////////////////////////////////////////////
            services.ConfigureCors();
            services.ConfigureIISIntegration();
            services.ConfigureLogger();
            services.ConfigureSqlContext(Configuration);
            services.ConfigureRepositoryManager();
            services.AddAutoMapper(typeof(Startup));

            services.AddResponseCaching();

            services.AddResponseCompression();

            services.AddMemoryCache();

            services.AddScoped<ValidationFilterAttribute>();
            services.AddScoped<ValidateCompanyExistsAttribute>();

            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = new List<RateLimitRule> {
                    new RateLimitRule() { Endpoint = "*", Limit = 50, PeriodTimespan = new TimeSpan(0, 1, 0) }
                };
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


            services.AddHttpContextAccessor();

            services.AddAuthentication("Bearer")
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = "https://localhost:5001/";
                options.RequireHttpsMetadata = true;
                options.ApiName = "WebApi1";
                options.EnableCaching = false;
                options.JwtValidationClockSkew = TimeSpan.Zero;
            });
            ////////////////////////////////////////////////////////////////////
            #endregion ServiceExtentions

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.ConfigureExceptionHandler(logger);
            }

            app.UseSerilogRequestLogging();


            app.UseHttpsRedirection();



            app.UseStaticFiles();
            app.UseCors("CorsPolicy");


            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
            });

            app.UseResponseCaching();
            app.UseIpRateLimiting();
            app.UseResponseCompression();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<LogUserNameMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
