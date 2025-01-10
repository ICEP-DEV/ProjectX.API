using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model.dto
{
    public class StatusDTO
    {
        public int AlumnusId { get; set; }
        public int EventId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
