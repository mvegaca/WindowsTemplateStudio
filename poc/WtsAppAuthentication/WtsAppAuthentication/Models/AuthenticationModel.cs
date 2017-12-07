namespace WtsAppAuthentication.Models
{
    public class AuthenticationModel
    {
        public bool RememberCredentials { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
