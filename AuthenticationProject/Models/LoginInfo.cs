namespace AuthenticationProject.Models
{
    public class LoginInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserId { get; set; }
        public DateTime LastLoginDateTime { get; set; }
    }

}
