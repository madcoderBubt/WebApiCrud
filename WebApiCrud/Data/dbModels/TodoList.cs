using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCrud.Data.dbModels
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsDeleted { get; set; }

        public int TodoListId { get; set; }
        public TodoList TodoList { get; set; }
    }
    public class TodoList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public string UserName { get; set; } //User Name of Todo List creator
        public List<Todo> Todos { get; set; }
    }
}
