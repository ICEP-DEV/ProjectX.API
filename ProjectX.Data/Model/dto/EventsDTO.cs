using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Data.Model.dto
{
    public class EventsDTO
    {
       
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public string[] VolunteerRoles { get; set; }
        public string Media { get; set; } = string.Empty; // Base64 string
    }
}
