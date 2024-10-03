namespace ProjectX.Data.Model.dto
{
    public class LoginDTO
    {
        public int UserId { get; set; }

        public string Password { get; set; }

        public string Role { get; set; } // 'admin' or 'alumni'
    }
}
