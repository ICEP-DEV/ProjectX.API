using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model
{
    public class News
    {
        public int Id { get; set; }
        public string NewsType { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string PublishedDate { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public byte[] Media { get; set; } = Array.Empty<byte>();  // Default to an empty byte array

    }
}
