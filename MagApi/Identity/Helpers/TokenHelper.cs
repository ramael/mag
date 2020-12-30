using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace MagApi.Identity.Helpers
{
    public class TokenHelper : ITokenHelper
    {
        private readonly IConfiguration _config;

        public TokenHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string username, IEnumerable<string> roles)
        {
            var claimsIdentity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, username)
            });

            if (roles != null && roles.Count() > 0) {
                foreach (string role in roles) {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }

            var key = _config.GetValue<string>("Jwt:Key");
            var issuer = _config.GetValue<string>("Jwt:Issuer");
            var audience = _config.GetValue<string>("Jwt:Audience");
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.Now.AddMinutes(_config.GetValue<int>("Jwt:Expires")),
                SigningCredentials = signingCredentials,

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
    }

}
