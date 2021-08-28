using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [ApiController]
    [Route("[controller]")]
    [Authorize]
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
            IEnumerable<TodoList> data = dbContext.TodoLists
                .Where(f=>f.UserName == User.Identity.Name && f.IsDeleted == false).ToList();

            return Ok(data);
        }
        [HttpGet]
        [Route("Trash")]
        public async Task<IActionResult> GetTrashAsync()
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanGet)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to get Data." });
            IEnumerable<TodoList> data = dbContext.TodoLists
                .Where(f => f.UserName == User.Identity.Name && f.IsDeleted == true).ToList();

            return Ok(data);
        }

        // GET api/<TodoController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanGet)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to get Data." });
            var data = dbContext.Todos.Where(s => s.TodoListId == id).ToList();
            return Ok(data);
        }

        // POST api/<TodoController>
        [HttpPost]
        [Route("NewTodoList")]
        public async Task<IActionResult> NewTodoListAsync([FromBody] TodoList todoList)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanPost)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to post Data." });
            if(todoList == null) return StatusCode(StatusCodes.Status204NoContent, new { Status = "Error", Message = "Null Reference!" });

            //dbContext
            try
            {
                TodoList data = new TodoList()
                {
                    Id = 0,
                    Name = todoList.Name,
                    IsDeleted = false,
                    UserName = User.Identity.Name
                };
                dbContext.TodoLists.Add(data);
                await dbContext.SaveChangesAsync();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Something went wrong!" });
            }
        }
        [HttpPost]
        [Route("NewTodo")]
        public async Task<IActionResult> NewTodoAsync(int? todoListId,[FromBody] Todo todo)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanPost)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to post Data." });
            if (todo == null) return StatusCode(StatusCodes.Status204NoContent, new { Status = "Error", Message = "Null Reference!" });

            //dbContext
            try
            {
                Todo data = new Todo()
                {
                    Id = 0,
                    Title = todo.Title,
                    IsDeleted = false,
                    IsCompleted = false,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    TodoListId = todo.TodoListId
                };
                dbContext.Todos.Add(data);
                await dbContext.SaveChangesAsync();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Something went wrong!" });
            }
        }

        // PUT api/<TodoController>/5
        [HttpPut("{id}")]
        [Route("Edit")]
        public async Task<IActionResult> PutTodoAsync(int? id, [FromBody] Todo todo)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanPut)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to post Data." });

            try
            {
                Todo data = dbContext.Todos.Find(todo.Id);
                {
                    //data.Id = todo.Id,
                    data.Title = todo.Title;
                    data.Description = todo.Description;
                    data.DueDate = todo.DueDate;
                }
                dbContext.Todos.Update(data);
                await dbContext.SaveChangesAsync();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Something went wrong!" });
            }
            //return Ok();
        }
        [HttpPut("{id}")]
        [Route("Done")]
        public async Task<IActionResult> DoneTodoAsync(int? id, [FromBody] DoneViewModel model)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanPut)
                return Unauthorized(new { Status = "Error", Message = "You have no permission to post Data." });
            try
            {
                Todo data = dbContext.Todos.Find(model.TodoId);
                {
                    //data.Id = todo.Id,
                    data.IsCompleted = model.IsDone;
                }
                dbContext.Todos.Update(data);
                await dbContext.SaveChangesAsync();
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "Something went wrong!" });
            }
            //return Ok();
        }

        // DELETE api/<TodoController>/5
        [Route("DelTodoList/{id:int}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoListAsync(int id)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanDelete)
                return Unauthorized(new { success = false, Status = "Error", Message = "You have no permission to post Data." });
            
            try
            {
                var data = dbContext.TodoLists.Find(id);
                data.IsDeleted = true;
                dbContext.TodoLists.Update(data);
                await dbContext.SaveChangesAsync();

                return Ok(new { success = true, Status = "Ok", Message="Done" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, Status = "Error", Message = "Something went wrong!" });
            }
        }
        [HttpDelete("{id}")]
        [Route("DelTodoItem/{id:int}")]
        public async Task<IActionResult> DeleteTodoItemAsync(int id)
        {
            if (!User.IsInRole("Admin") &&
                !(await userManager.FindByNameAsync(User.Identity.Name)).CanDelete)
                return Unauthorized(new { success = false, Status = "Error", Message = "You have no permission to post Data." });

            try
            {
                var data = dbContext.Todos.Find(id);
                //data.IsDeleted = true;
                //dbContext.Todos.Update(data);
                dbContext.Todos.Remove(data);
                await dbContext.SaveChangesAsync();

                return Ok(new { success = true, Status = "Ok", Message = "Done" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, Status = "Error", Message = "Something went wrong!" });
            }
        }
    }
}
