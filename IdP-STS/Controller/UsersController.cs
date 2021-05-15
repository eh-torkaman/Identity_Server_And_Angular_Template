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
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Http.Headers;

namespace STS.Controller
{
  [Route("api/users")]
  [ApiController]
  //[Authorize(Roles = "SuperAdmin")]
  [Authorize(Roles = "SuperAdmin", AuthenticationSchemes = "Bearer")]
  public class UsersController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> userMgr;
    private readonly ApplicationDbContext db;

    public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
      this.userMgr = userManager;
      this.db = db;
    }

    //[Route("/api/UsersAndClaims")]
    //[HttpGet]
    //public IActionResult GetAllUsersAndClaims()
    //{
    //    return Ok(db.VUsersClaims.ToList());
    //}

    [HttpGet]
    public IActionResult GetAllUsers()
    {
      return Ok(db.dbUsers.Include(t => t.dbClaims).ToList().OrderByDescending(t => t.IsLockedOut).ThenBy(t => t.UserName).ToList());
    }

    [HttpPost]
    public IActionResult CreateUser(UserForCreationDto us)
    {
      try
      {
        var user = userMgr.FindByNameAsync(us.UserName).Result;
        if (user != null)
        {
          return BadRequest("این کاربر قبلاً وجود دارد");
        }

        user = new ApplicationUser { UserName = us.UserName, Email = "", EmailConfirmed = true, };

        var result = userMgr.CreateAsync(user, us.Password).Result;
        if (!result.Succeeded)
        {
          throw new Exception(result.Errors.First().Description);
        }

        Log.Debug($" کاربر { us.UserName} ساخته شد");

        return Ok();
      }
      catch (Exception ee)
      {
        Log.Error(ee.Message);
        return BadRequest(ee.Message);
      }
    }


    [HttpPut]
    public IActionResult ResetUserPassword(UserForCreationDto us)
    {
      try
      {
        var user = userMgr.FindByNameAsync(us.UserName).Result;
        if (user == null)
        {
          return BadRequest("این کاربر وجود ندارد");
        }
        if (user.UserName.ToLower() == "SuperAdmin".ToLower())
        {
          return BadRequest("برای این کاربر شدنی نیست");
        }

        var token = userMgr.GeneratePasswordResetTokenAsync(user).Result;

        var result = userMgr.ResetPasswordAsync(user, token, us.Password).Result;
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


    [HttpDelete("{UserName}")]
    public IActionResult DeleteUser(string userName)
    {
      try
      {
        var user = userMgr.FindByNameAsync(userName).Result;
        if (user == null)
        {
          return BadRequest("این کاربر وجود ندارد");
        }
        if (userName.ToLower() == "SuperAdmin".ToLower())
        {
          return BadRequest("این کاربر قابل پاک شدن نیست");
        }

        var result = userMgr.DeleteAsync(user).Result;
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

    [HttpPut("{userName}")]
    [Authorize( AuthenticationSchemes = "Bearer")]
    public IActionResult ChangeUserPass(string userName, ChangeUserPassDto changeUserPassDto)
    {
      try
      {
        if (HttpContext.User.Identity.Name != userName)
        {
          throw new Exception("تنها می توانید پسورد خودتان را عوض کنید");
        }
        var user = userMgr.FindByNameAsync(userName).Result;
        if (user == null)
        {
          return BadRequest("این کاربر وجود ندارد");
        }
        var result = userMgr.ChangePasswordAsync(user, changeUserPassDto.OldPassword, changeUserPassDto.NewPassword).Result;
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

    [HttpPut("{userName}/Lock")]
    public IActionResult ChangeUserPass(string userName)
    {
      try
      {
        var user = userMgr.FindByNameAsync(userName).Result;
        if (user == null)
        {
          return BadRequest("این کاربر وجود ندارد");
        }



        if ((user.LockoutEnd != null) && (user.LockoutEnd > DateTimeOffset.Now))
          user.LockoutEnd = DateTimeOffset.Now;
        else
        {
          if (userName.ToLower() == "SuperAdmin".ToLower())
            return BadRequest("این کاربر قابل قفل شدن نیست");
          user.LockoutEnd = DateTimeOffset.Now.AddYears(10);
        }

        var result = userMgr.UpdateAsync(user).Result;
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


    [HttpPost("{userName}/uploadImage"), RequestSizeLimit(bytes: 500 * 1024)]
    public async Task<IActionResult> Upload(string userName)
    {
      try
      {
        //   var file = Request.Form.Files[0];
        var formCollection = await Request.ReadFormAsync();
        var file = formCollection.Files.First();

        var folderName = Path.Combine("Resources", "usersImages");
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        if (file.Length > 0)
        {
          var fullfn = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var fileExtension = Path.GetExtension(fullfn).ToLower();

          if (fileExtension != ".jpg")
            return BadRequest("فقط تصاویر jpg پشتیبانی میشود");

          var fileName = userName + fileExtension;

          var fullPath = Path.Combine(pathToSave, fileName);
          using (var stream = new FileStream(fullPath, FileMode.Create))
          {
            await file.CopyToAsync(stream);
          }

          var userToChangeImage = await userMgr.FindByNameAsync(userName);
          userToChangeImage.ProfileImageNumber = new Random().Next();
          await userMgr.UpdateAsync(userToChangeImage);

          return Ok(new KeyValuePair<string, int>("ProfileImageNumber", userToChangeImage.ProfileImageNumber));
        }
        else
        {
          return BadRequest();
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }

  }

}
