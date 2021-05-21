using IdP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using STS.Data;
using STS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdP.Controller
{
    [Route("api/Roles")]
    [ApiController]
    //[Authorize(Roles = "SuperAdmin", AuthenticationSchemes = "Bearer")]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userMgr;
        private readonly ApplicationDbContext db;
        private readonly RoleManager<IdentityRole> roleManager;

        public RolesController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            this.userMgr = userManager;
            this.db = db;
            this.roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<String>> GetAllRoles() => Ok(db.Roles.Select(t => t.Name).ToList());


        [HttpPost("{roleName}")]
        public async Task<ActionResult> AddRole(string roleName)
        {
            try
            {
                roleName = roleName.TrimEvelNull();
                if ((roleName=="")||(roleName != roleName.ReplaceAllNonAlphaNumericExceptAllowableListOFCharacters(allowableListOFCharacters:"_")))
                    throw new Exception("این نام نقش مجاز نمی باشد");

                bool x = await roleManager.RoleExistsAsync(roleName);
                if (!x)
                {
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                        throw new Exception(result.Errors.First().Description);
                }
                else
                {
                    var rs = new ListOfCustomMessages(msg: "این نقش قبلا وجود دارد", msgTypeEnum : MsgTypeEnum.Error);
                    return BadRequest(rs);
                }

                return Ok(new ListOfCustomMessages("نقش "+ roleName+" ساخته شد",msgTypeEnum:MsgTypeEnum.Success));
            }
            catch (Exception ee)
            {
                Log.Error(ee.Message);
                return BadRequest(new ListOfCustomMessages(ee));
            }
        }

        [HttpDelete("{roleName}")]
        public async Task<ActionResult> DeleteRole(string roleName)
        {
            try
            {
                var role = await roleManager.FindByNameAsync(roleName);
                
                if (role==null)
                {
                    var rs = new ListOfCustomMessages(msg: "این نقش وجود ندارد", msgTypeEnum: MsgTypeEnum.Error);
                    return NotFound(rs);
                }

                var result = await roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                return Ok(new ListOfCustomMessages("نقش "+ roleName+" حذف شد", MsgTypeEnum.Success));
            }
            catch (Exception ee)
            {
                Log.Error(ee.Message);
                return BadRequest(new ListOfCustomMessages(ee));
            }
        }
    }


}
