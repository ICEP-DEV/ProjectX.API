using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TUTDb.Data.Model
{
    public class Alumni
    {
        public Alumni()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Course = string.Empty;
            Campus = string.Empty;
            Faculty = string.Empty;
        }

        [Key]
        public int id { get; set; } 
        public int AlumnusId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Course { get; set; }
        public int GraduationYear { get; set; }
        public string Campus { get; set; }
        public string Faculty { get; set; }
        

    }
}
