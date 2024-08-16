namespace GarageCoreAPI.Models
{
    public class AuthResponseMessage
    {
        public bool LoggedIn { get; set; } = false;
        public bool Registered { get; set; } = false;
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
