using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCrud.Data;
using WebApiCrud.Data.dbModels;
using WebApiCrud.Data.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiCrud.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        public readonly ApplicationDbContext dbContext;
        public readonly UserManager<AppUser> userManager;
        public AdminController(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        // GET: api/<AdminController>
        [HttpGet]
        public IEnumerable<UserViewModel> Get()
        {
            //var exceptUser = us
            IEnumerable<UserViewModel> users = dbContext.Users.ToList().Select(s => new UserViewModel() { 
                UserName = s.UserName, CanDelete = s.CanDelete, CanGet = s.CanGet, CanPost = s.CanPost, CanPut = s.CanPut
            });
            return users;
        }


        // POST api/<AdminController>
        [HttpPost]
        public void Post([FromBody] UserViewModel value)
        {

        }

        // PUT api/<AdminController>/5
        [HttpPut("{id}")]
        [Route("UserAccess")]
        public async Task<IActionResult> PutUserAccessAsync(int? id, [FromBody] UserViewModel value)
        {
            var user = await userManager.FindByNameAsync(value.UserName);
            user.CanGet = value.CanGet;
            user.CanPost = value.CanPost;
            user.CanPut = value.CanPut;
            user.CanDelete = value.CanDelete;
            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "unknown" });
            }
        }

        // DELETE api/<AdminController>/5
        [HttpDelete("{id}")]
        public async Task DeleteAsync(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            await userManager.DeleteAsync(user);
        }
    }
}
