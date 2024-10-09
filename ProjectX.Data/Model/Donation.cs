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
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        public string Event{ get; set; }

    }
}
