using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Saylo.Centrex.Identity.Core.Infrastructure.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Saylo.Centrex.Identity.Core.Infrastructure.Services
{
    public interface ITokenService
    {
        string GenerateToken(IEnumerable<Claim> claims);
    }
    public class ClaimNames 
    {
        public const string Sub = "sub";
        public const string Jti = "Jti";
        public const string UserName = "UserName";
        public const string TenantId = "TenantId";

    }
    public class JwtService: ITokenService
    {
        private readonly ICertificateService _certificateService;
        private readonly AppSettings _appSettings;
        

        public JwtService(ICertificateService certificateService ,AppSettings appSettings)
        {
            _certificateService = certificateService;
            _appSettings = appSettings;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var cert = _certificateService.GetCurrentCertificate();
            var rsaKey = new RsaSecurityKey(cert.Certificate.GetRSAPrivateKey())
            {
                KeyId = cert.Kid
            };
            var creds = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _appSettings.OpenIdSettings.Issuer,
                audience: _appSettings.OpenIdSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_appSettings.OpenIdSettings.jwtExpiresMinute)),
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }   
}
