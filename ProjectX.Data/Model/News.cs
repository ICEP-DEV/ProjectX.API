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
        public string Headline { get; set; }
        public string Description { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public byte[] Media { get; set; } = Array.Empty<byte>();  // Default to an empty byte array

    }
}
