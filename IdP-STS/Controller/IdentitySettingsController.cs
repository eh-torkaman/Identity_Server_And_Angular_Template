using IdP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using STS.Data;
using STS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdP.Controller
{
    [Route("api/Settings")]
    [ApiController]
    public class IdentitySettingsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userMgr;
        private readonly ApplicationDbContext db;

        public IdentitySettingsController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            this.userMgr = userManager;
            this.db = db;
        }
     
        [HttpGet("password/Options")]
        [AllowAnonymous]
        public ActionResult<CustomPasswordOptions> GetCustomPasswordOptions() => Ok(db.CustomPasswordOptions.OrderBy(t => t.Id).LastOrDefault());


        [HttpPost("password/Options")]
        [Authorize(Roles = "SuperAdmin", AuthenticationSchemes = "Bearer")]
        public ActionResult SetCustomPasswordOptions(CustomPasswordOptions customPasswordOptions)
        {
            var cpo = db.CustomPasswordOptions.OrderBy(t => t.Id).LastOrDefault();
            customPasswordOptions.Id = cpo.Id;
            db.Entry(customPasswordOptions).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

       


    }
}
