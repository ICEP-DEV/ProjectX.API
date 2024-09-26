using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model
{
    public class Admin
    {
        public Admin() {
            Name = string.Empty;
            Password = string.Empty;
        }

        public int AdminId { get; set; }//staff number

        public string Name { get; set; }
        public string Password { get; set; }
    }
}
