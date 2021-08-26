using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCrud.Data;
using WebApiCrud.Data.dbModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiCrud.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        public readonly ApplicationDbContext dbContext;
        public readonly UserManager<AppUser> userManager;

        public TodoController(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        // GET: api/<TodoController>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanGet)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to get Data." });
            IEnumerable<TodoList> data = dbContext.TodoLists.Include(s => s.Todos).ToList();

            return Ok(data);
        }

        // GET api/<TodoController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanGet)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to get Data." });
            TodoList data = dbContext.TodoLists.Include(s => s.Todos).SingleOrDefault(s => s.Id == id);
            return Ok(data);
        }

        // POST api/<TodoController>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] string value)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanPost)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to post Data." });
            return Ok();
        }

        // PUT api/<TodoController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] string value)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanPut)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to post Data." });
            return Ok();
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanDelete)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to post Data." });
            return Ok();
        }
    }
}
