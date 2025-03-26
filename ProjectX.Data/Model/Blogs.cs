using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model
{
    public  class Blogs
    {
        public Blogs() { 

            Name = String.Empty;
            Role = String.Empty;
            Description = String.Empty;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; } = Array.Empty<byte>();  // Default to an empty byte array
    }
}
