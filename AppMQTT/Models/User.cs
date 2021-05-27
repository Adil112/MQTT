using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppMQTT.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
