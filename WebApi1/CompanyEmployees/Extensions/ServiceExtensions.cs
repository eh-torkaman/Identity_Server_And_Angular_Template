using CompanyEmployees.InputOutputFormatter;
using Contracts;
using Entities;
using LoggerManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Extensions
{
    public static class ServiceExtensions
    {



        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
            builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));


        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        }
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(opt =>
            opt.EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseSqlServer(configuration.GetConnectionString("sqlConnection"),
            sqlServerDbContextOptionsBuilder =>
            {
                sqlServerDbContextOptionsBuilder.MigrationsAssembly("CompanyEmployees");

            })
            );
        }
        public static void ConfigureLogger(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager>(x => new LoggerManager.LoggerManager(Serilog.Log.Logger));
            services.AddSingleton(typeof(ILoggerManager<>), typeof(LoggerManager.LoggerManager<>));
        }
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(setupAction =>
            {
                setupAction.AddPolicy("CorsPolicy", corsPolicybuilder =>
                {
                    corsPolicybuilder.AllowAnyOrigin();
                    corsPolicybuilder.AllowAnyMethod();
                    corsPolicybuilder.AllowAnyHeader();
                });
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(option =>
            {
            });
        }
    }
}
