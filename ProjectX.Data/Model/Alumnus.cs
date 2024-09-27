using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model
{
    public class Alumnus
    {
        public Alumnus() 
        {
           
            Password = string.Empty;
        }

        public int Id { get; set; }
        [Key]
        public int AlumnusId { get; set; }//student number
  
        public string Password { get; set; }
    }
}
