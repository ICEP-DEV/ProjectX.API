using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model.dto
{
    public class Response
    {
        public int StatusCode { get; set; }
        
        public string StatusMessage { get; set; } = string.Empty;

        public Admin admin { get; set; }

        public List<Alumnus> listAlumnus { get; set; }

        

    }
}
