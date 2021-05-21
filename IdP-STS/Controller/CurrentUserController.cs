using IdentityServer4.Models;
using IdP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using STS.Data;
using STS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdP.Controller
{
    [Route("api/CurrentUser")]
    [ApiController]
    [Authorize( AuthenticationSchemes = "Bearer")]
    public class CurrentUserController : ControllerBase
    {
         
        private readonly UserManager<ApplicationUser> userMgr;
        private readonly ApplicationDbContext db;

        private string userName { get { return (HttpContext.User?.Identity?.Name)??"BadUser"; } }
        public CurrentUserController(UserManager<ApplicationUser> userManager, ApplicationDbContext db )
        {
            this.userMgr = userManager;
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetUser()
        {
            var rs = db.dbUsers.Include(t => t.dbClaims).First(t => t.UserName == this.userName);
            return Ok(rs);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public ActionResult ChangeUserPass(ChangeUserPassDto changeUserPassDto)
        {
            try
            {
                var user = userMgr.FindByNameAsync(userName).Result;
                var result = userMgr.ChangePasswordAsync(user, changeUserPassDto.OldPassword, changeUserPassDto.NewPassword).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                var rs= new CustomMessages() {new CustomMessage { Message = "رمز عبور تغییر کرد" } };
                return Ok( rs);
            }
            catch (Exception ee)
            {
                Log.Error(ee.Message);
                var tt0 = new CustomMessage { Message = "رمز عبور تغییر نکرد", MsgTypeEnum = MsgTypeEnum.Warning };
                var rs = new CustomMessages() {new CustomMessage (ee), tt0 };
                return BadRequest(rs);
            }
        }

    }
}
