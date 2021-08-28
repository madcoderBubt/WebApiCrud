using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCrud.Data.Models
{
    public class DoneViewModel
    {
        public int TodoId { get; set; }
        public bool IsDone { get; set; }
    }
}
