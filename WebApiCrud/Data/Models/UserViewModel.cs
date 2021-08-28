using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCrud.Data.Models
{
    public class UserViewModel
    {
        [Required]
        public string UserName { get; set; }
        //public List<string> Roles { get; set; }
        public bool CanGet { get; set; }
        public bool CanPost { get; set; }
        public bool CanPut { get; set; }
        public bool CanDelete { get; set; }
    }
}
