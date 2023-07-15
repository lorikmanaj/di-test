namespace AMS.Web.ViewModels.Results
{
    public class AuthenticateResult
    {
        public bool Succeeded { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
    }
}
