// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using STS.Data;
using STS.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityServer4.Services;
using System;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace STS
{
  public class Startup
  {
    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    {
      Environment = environment;
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews()
           .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


      services.AddCors(
        sa => sa.AddPolicy("Cors", builder => builder
                                                    .WithOrigins("http://localhost:4200")
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod())
      );

      services.AddFormOptionConfiguration();

      services.AddTransient<IProfileService, CustomProfileService>();

      services.AddDbContext<ApplicationDbContext>(options =>
      {
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sqlServerOptionsAction => { sqlServerOptionsAction.CommandTimeout(20); });
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
      });

      services.AddIdentity_ConfigureApplicationCookie();

      services.AddIdentityServerStuff();

      services.AddAuthentication("Bearer")
             .AddIdentityServerAuthentication(options =>
             //.AddJwtBearer(options =>
             {
                 options.Authority = "https://localhost:5001/";
                 options.RequireHttpsMetadata = false;
                 options.ApiName = "IdPApi";
                 //  options.se = "IdPApi_secret";
               });
    }

    public void Configure(IApplicationBuilder app)
    {
      if (Environment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        // app.UseDatabaseErrorPage();
      }
      app.UseStaticFiles();
      app.UseStaticFiles(new StaticFileOptions()
      {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources/usersImages")),
        RequestPath = new PathString("/userImage")
      });

      app.UseRouting();

      app.UseCors("Cors");

      app.UseIdentityServer();
      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapDefaultControllerRoute();
      });
    }
  }
}
