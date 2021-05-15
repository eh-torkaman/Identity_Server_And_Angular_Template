using STS.Data;
using STS.Models;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace STS.Controller
{
    [Route("api/claims/{userName}")]
    [ApiController]
    [Authorize(Roles ="SuperAdmin", AuthenticationSchemes = "Bearer")]
    public class UserClaimsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userMgr;
        private readonly ApplicationDbContext db;

        public UserClaimsController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            this.userMgr = userManager;
            this.db = db;
        }
        [HttpGet]
        public IActionResult GetAllClaims(string userName)
        {
            var user = userMgr.FindByNameAsync(userName).Result;
            if (user == null)
            {
                return NotFound("این کاربر وجود ندارد");
            }
            var claims = userMgr.GetClaimsAsync(user).Result;
            return Ok(claims);

        }

        [HttpPost]
        public IActionResult CreateClaim(ClaimDto claim, string userName)
        {
            try
            {
                if (claim.ClaimType == "" || claim.ClaimValue == "")
                    return BadRequest("هر دو متغیر نوع و مقدار یک کلیم باید غیر خالی باشند");
                var user = userMgr.FindByNameAsync(userName).Result;
                if (user == null)
                {
                    return NotFound("این کاربر وجود ندارد");
                }

                var cl = new System.Security.Claims.Claim(claim.ClaimType, claim.ClaimValue);

                if (db.VUsersClaims
                    .Any(t =>
                    (t.UserName.ToLower() == userName.ToLower() &&
                    (t.ClaimType.ToLower() == claim.ClaimType.ToLower())
                    ))
                    )
                {
                    return BadRequest("این کلیم برای این کاربر قبلا موجود می باشد ، ابتدا آن را پاک کنید");
                }

                var result = userMgr.AddClaimAsync(user, cl).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                return Ok();
            }
            catch (Exception ee)
            {
                Log.Error(ee.Message);
                return BadRequest(ee.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteAllClaimsWithGivenType(string userName, ClaimDto claim)
        {
            try
            {
                var user = userMgr.FindByNameAsync(userName).Result;
                if (user == null)
                {
                    return NotFound("این کاربر وجود ندارد");
                }

                var claimsToDelete = db.VUsersClaims.Where(t => t.UserName.ToLower() == userName.ToLower()).Where(t => t.ClaimType.ToLower() == claim.ClaimType)
                    .ToList().Select(t => new System.Security.Claims.Claim(type: claim.ClaimType, value: t.ClaimValue)).ToList();

                if (claimsToDelete.Count() == 0)
                {
                    return NotFound("چیزی پیدا نشد");
                }
                var result = userMgr.RemoveClaimsAsync(user, claimsToDelete).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                return Ok();
            }
            catch (Exception ee)
            {
                Log.Error(ee.Message);
                return BadRequest(ee.Message);
            }
        }



    }

}
