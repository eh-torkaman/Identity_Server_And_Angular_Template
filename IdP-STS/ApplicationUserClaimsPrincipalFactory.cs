using STS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Security.Claims;


namespace STS
{
    public class ApplicationUserClaimsPrincipalFactory :
      UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options
            ) : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity>
            GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (user?.UserName.ToLower()== "SuperAdmin".ToLower())
              identity.AddClaim(new Claim("role", "SuperAdmin"));

            return identity;
        }
    }
}
