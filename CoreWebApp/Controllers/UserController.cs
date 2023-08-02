using CoreWebApp.Models;
using JsonWebTokens.Models;
using JWTWebAuthentication.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoreWebApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //For admin Only
        [HttpGet]
        [Route("Admins")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminEndPoint()
        {
            var currentUser = GetCurrentUser();
            return Ok(currentUser);
            //return Ok($"Hi you are {currentUser.Username} with mail id {currentUser.Email} working in {currentUser.OfficeName} as an {currentUser.Role}  {currentUser.IsActive}");
        }
        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                //var officename = userClaims.FirstOrDefault(OfficeName)?.Value;
                return new UserModel
                {
                    Username = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Password = userClaims.FirstOrDefault(x => x.Type == "Password")?.Value,
                    Role = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
                    Email = userClaims.FirstOrDefault(x => x.Type == "Email")?.Value,
                    OfficeName = userClaims.FirstOrDefault(x => x.Type == "OfficeName")?.Value,
                    IsActive = Convert.ToBoolean(userClaims.FirstOrDefault(x => x.Type == "IsActive")?.Value)
                };
            }
            return null;
        }
    }
}
