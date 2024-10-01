using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model
{
    public class Alumnus
    {
        public Alumnus()
        {
            Email = string.Empty;
            Password = string.Empty;
        }
        public int Id { get; set; }

        public int AlumnusId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        public int isActive { get; set; } = 1;

        public DateTime CreatedOn { get; set; } = DateTime.Now;


    }
}
