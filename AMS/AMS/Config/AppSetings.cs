namespace AMS.Config
{
    public class AppSetings
    {
        public string JwtSecret { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
        public int TokenExpirationHours { get; set; }
    }
}
