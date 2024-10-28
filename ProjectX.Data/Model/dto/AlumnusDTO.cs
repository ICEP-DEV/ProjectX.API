namespace ProjectX.Data.Model.dto
{
    public class AlumnusDTO
    {
        public int StudentNum { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int ItsPin { get; set; }

        /*Token properties for password reset
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }*/
    }
}
