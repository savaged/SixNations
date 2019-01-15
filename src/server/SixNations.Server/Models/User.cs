namespace SixNations.Server.Models
{
    public class User : ModelBase
    {
        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string access_token { get; set; }
    }
}
