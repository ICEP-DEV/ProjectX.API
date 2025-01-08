using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model.dto
{
    public class VolunteerDTO
    {
        public int EventId { get; set; } // ID of the event

        public int AlumnusId { get; set; }

        //volunteer role
        public string Role { get; set; }
    }
}
