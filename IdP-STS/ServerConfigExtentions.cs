using STS.Data;
using STS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using IdP;
using System.Linq;
using System;
using IdP.Models;

namespace STS
{
  public static class ServerConfigExtentions
  {
    public static void AddIdentityServerStuff(this IServiceCollection services)
    {
      services.AddIdentityServer(options =>
      {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
        // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
        options.EmitStaticAudienceClaim = true;
        options.Authentication.CookieLifetime = TimeSpan.FromMinutes(60);
        options.Authentication.CookieSlidingExpiration = false;
      })
          .AddInMemoryIdentityResources(Config.IdentityResources)
          .AddInMemoryApiScopes(Config.ApiScopes)
          .AddInMemoryClients(Config.Clients)
          .AddInMemoryApiResources(Config.ApiResources)
          .AddAspNetIdentity<ApplicationUser>()
          .AddProfileService<CustomProfileService>().AddDeveloperSigningCredential();
    }

    public static void AddFormOptionConfiguration(this IServiceCollection services)
    {
      int threshHold = 1024 * 1024 * 10;
      services.Configure<FormOptions>(o =>
      {
        o.ValueLengthLimit = threshHold;// int.MaxValue;
        o.MultipartBodyLengthLimit = threshHold;//int.MaxValue;
        o.MemoryBufferThreshold = threshHold;//int.MaxValue;
      });
    }
    public static IServiceCollection AddIdentity_ConfigureApplicationCookie(this IServiceCollection services)
    {

      CustomPasswordOptions customPasswordOption = new ();
      
      try
      {
        using (var serviceProvider = services.BuildServiceProvider())
        {
          using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
          {
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            customPasswordOption = context.CustomPasswordOptions.OrderBy(t => t.Id).LastOrDefault();
          }
        }
      }
      catch (Exception ee)
      {

      }
      customPasswordOption = customPasswordOption == null ? new CustomPasswordOptions()
      {
        RequireDigit = false,
        RequiredLength = 2,
        RequiredUniqueChars = 1,
        RequireLowercase = false,
        RequireNonAlphanumeric = false,
        RequireUppercase = false

      } : customPasswordOption;

      services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
      {
        opt.SignIn = new SignInOptions() { RequireConfirmedEmail = false, RequireConfirmedAccount = false, RequireConfirmedPhoneNumber = false };
        opt.Password =  customPasswordOption;
        opt.Lockout = new LockoutOptions()
        {
          AllowedForNewUsers = true,
          DefaultLockoutTimeSpan = new System.TimeSpan(hours: 0, minutes: 3, seconds: 0),
          MaxFailedAccessAttempts = 10
        };
      //  opt.Stores = new StoreOptions() { MaxLengthForKeys = 450, ProtectPersonalData = false };
        //opt.User = new UserOptions() { AllowedUserNameCharacters = "abcdefg...", RequireUniqueEmail = true };
      })
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders()
          .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
          ;

      services.ConfigureApplicationCookie(configure =>
      {

        configure.Events.OnRedirectToLogin = context =>
        {
          if (context.Request.Path.ToString().ToLower().Contains("/api/"))
          {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
          }

          context.Response.Redirect(context.RedirectUri);
          return Task.CompletedTask;
        };


        configure.Events.OnRedirectToAccessDenied = context =>
        {
          if (context.Request.Path.ToString().ToLower().Contains("/api/"))
          {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
          }

          context.Response.Redirect(context.RedirectUri);
          return Task.CompletedTask;
        };
      });
      return services;
    }

  }

}
