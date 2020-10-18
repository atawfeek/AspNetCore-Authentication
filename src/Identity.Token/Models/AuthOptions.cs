namespace AspNetCore.Authentication.Identity.Token.Models
{
    public class AuthOptions
    {
        public const string Auth = "Auth";

        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SymmetricSecurityKey { get; set; }
    }
}
