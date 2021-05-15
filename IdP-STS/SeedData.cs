// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using STS.Data;
using STS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace STS
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));

            services.AddIdentity_ConfigureApplicationCookie();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var SuperAdmin = userMgr.FindByNameAsync("SuperAdmin").Result;
                    if (SuperAdmin == null)
                    {
                        SuperAdmin = new ApplicationUser
                        {
                            UserName = "SuperAdmin",
                            Email = "",
                            EmailConfirmed = true,
                        };
                        var result = userMgr.CreateAsync(SuperAdmin, "SuperAdmin").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("SuperAdmin created");
                    }
                    else
                    {
                        Log.Debug("SuperAdmin already exists");
                    }

                }
            }
        }
    }
}
