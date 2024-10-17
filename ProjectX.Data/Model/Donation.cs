using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model
{
    
        
        public class Donation
        {
            [Key]
            public int DonationId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;

            public string EventOptions { get; set; } // Save selected events as a comma-separated string
        }


    
}
