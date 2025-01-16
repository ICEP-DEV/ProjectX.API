using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectX.Data.Model.dto
{
    public class AlumnusDTO
    {
        public AlumnusDTO()
        {
            LinkedInProfile = string.Empty;

        }

        public int StudentNum { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int ItsPin { get; set; }

        public string LinkedInProfile { get; set; }

        public byte[]? ProfilePicture { get; set; }

        /*Token properties for password reset
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }*/
    }
}
