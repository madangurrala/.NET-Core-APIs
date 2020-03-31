using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RentSpace.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RentSpace.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly AppAuthentication appAuth;

        public AuthenticateService(IOptions<AppAuthentication> appAuth)
        {
            this.appAuth = appAuth.Value;
        }

        public string Authenticate(string email)
        { 
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appAuth.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, "Admin"),
                    new Claim(ClaimTypes.Version, "V3.1")
                }),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(createdToken);

            return token;
        }
    }
}
