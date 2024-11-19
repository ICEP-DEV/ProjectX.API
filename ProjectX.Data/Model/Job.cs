using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public string Faculty { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Vacancy { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Closingdate { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}
