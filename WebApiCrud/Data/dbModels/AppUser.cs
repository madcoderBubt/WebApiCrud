using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCrud.Data.dbModels
{
    public class AppUser:IdentityUser
    {
        public bool CanGet { get; set; }
        public bool CanPost { get; set; }
        public bool CanPut { get; set; }
        public bool CanDelete { get; set; }
    }
}
