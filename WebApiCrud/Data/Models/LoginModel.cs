using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCrud.Data.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is Required.")]
        public string UserName { get; set; }
        //[Required(ErrorMessage = "Email is Required.")]
        //[DataType(DataType.EmailAddress)]
        //public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is Required.")]
        public string Password { get; set; }
    }
}
