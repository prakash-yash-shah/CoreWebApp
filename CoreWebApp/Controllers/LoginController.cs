using CoreWebApp.ContextClass;
using CoreWebApp.Models;
using Dawn;
using JsonWebTokens.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace JsonWebTokens.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly BrandContext _db;
        private readonly IConfiguration _config;
        string token = null;
        string tokenvalue = null;
        public LoginController(IConfiguration config, BrandContext db)
        {
            _config = config;
            _db = db;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string username,string password)
        {
            UserLogin uslo = new UserLogin()
            {
                Username = username,
                Password = password,
            };
            var user = Authenticate(uslo);
            user = Guard.Argument(user, nameof(user)).NotNull().Value;
            if (user != null)
            {
                token = GenerateToken(user);
                return Ok(token);   
            }
            return NotFound("user not found");
        }

        private Users Authenticate(UserLogin userLogin)
        {
            var userid = _db.userLoginModels.Where(x=>x.Username == userLogin.Username).Select(x=>x.Loginid).FirstOrDefault();
            var users = _db.users.Where(x=>x.UserId == userid).FirstOrDefault();
            //var currentUser = UserConstants.Users.FirstOrDefault(x => x.Username.ToLower() ==
            //    userLogin.Username.ToLower() && x.Password == userLogin.Password);
            if (users != null)
            {
                return users;
            }

            return null;
        }
        // Token is Generated from here
        private string GenerateToken(Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim("Password",user.Password),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Email", user.Email),
                new Claim("OfficeName", user.OfficeName),
                new Claim("IsActive", user.ActiveStatus.ToString())
            };
            //var claims = new[]
            //{
            //    new Claim(ClaimTypes.NameIdentifier,user.Username),
            //    new Claim(ClaimTypes.Role,user.Role)
            //};
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

             tokenvalue = token.EncodedPayload.ToString();
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
