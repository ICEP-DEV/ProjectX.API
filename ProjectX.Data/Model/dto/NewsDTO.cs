using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model.dto
{
    public class NewsDTO
    {
        public int Id { get; set; }
        public string NewsType { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string PublishedDate { get; set; } = string.Empty;
        public string Media { get; set; } = string.Empty;  // Default to an empty byte array
    }
}
