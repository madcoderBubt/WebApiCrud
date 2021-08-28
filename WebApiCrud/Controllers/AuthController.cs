using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiCrud.Data.dbModels;
using WebApiCrud.Data.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiCrud.Controllers
{
    //[Route("api/[controller]/[action]")]
    [ApiController]
    //[EnableCors]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        public AuthController(UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("Auth/")]
        public IActionResult Get()
        {
            return Ok(new { Status = "test", Message = "User Exists!" });
        }

        [HttpPost]
        [Route("Auth/Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            
            var usrExists = await userManager.FindByNameAsync(model.UserName);
            if (usrExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User Exists!" });
            }
            AppUser user = new AppUser()
            {
                Email = model.Email,
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) //if unable to create user
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Unable to reg. plz check input data again." });
            else if (!await roleManager.RoleExistsAsync("Admin")) //if admin not exist create admin user
            {
                if ((await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" })).Succeeded)
                    await userManager.AddToRoleAsync(user, "Admin");
            }
            else //if admin role exists then create or asign user role
            {
                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
                await userManager.AddToRoleAsync(user, "User");
            }
            return Ok(new { Status = "Success", Message = "Registration Successed" });
        }

        [HttpPost]
        [Route("Auth/Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (model == null) return StatusCode(StatusCodes.Status204NoContent, new { Status = "Error", Message = "Null Reference!" });
            //if (!ModelState.IsValid) return Content();

            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var roles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach (var item in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, item));
                }
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: configuration["JWT:ValidIssuer"],
                    audience: configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new { status="Success", user=user.UserName, roles = roles,
                    token = new JwtSecurityTokenHandler().WriteToken(token), 
                    expiration = token.ValidTo });
            }
            
            return Unauthorized();
        }
    }
}
