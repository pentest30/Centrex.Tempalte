namespace Saylo.Centrex.Identity.Core.Infrastructure.Configuration
{

    public class AppSettings
    {
        public IdentitySettings IdentitySettings { get; set; }
        public OpenIdSettings OpenIdSettings { get; set; }

    }
    public class IdentitySettings
    {
        public int PasswordRequiredLength { get; set; }
        public bool PasswordIsRequireDigit { get; set; }
        public bool PasswordIsRequireLowercase { get; set; }
        public bool PasswordIsRequireNonAlphanumeric { get; set; }
        public bool PasswordIsRequireUppercase { get; set; }
        public int PasswordMaxFailedAccessAttempts { get; set; }

      

    }

    public class  OpenIdSettings
    {
        public string JwksUri { get; set; }//https://localhost:44322/.well-known/jwks.json
        public string IdentityUrl { get; set; }
        public string Issuer { get; set; } //"saylo.centrex.identity.api"

        public string Audience { get; set; } //"saylo.centrex"

        public string jwtExpiresMinute { get; set; } //"saylo.centrex"
        

    }
}
