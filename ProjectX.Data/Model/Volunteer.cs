using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model
{
    public class Volunteer
    {
        [Key]
        public int Id { get; set; } // Primary Key

        // Foreign Key referencing the Alumnus table
       
        [ForeignKey("AlumnusProfile")]
        public int AlumnusId { get; set; }

        // Foreign Key referencing the Events table
        [ForeignKey("Event")]
        public int EventId { get; set; }       

        //volunteer role
        public string Role { get; set; }
    }
}
