using AspNetCore.Authentication.Identity.Token.Interfaces;
using AspNetCore.Authentication.Identity.Token.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNetCore.Authentication.Identity.Token.Services
{
    public class TokenIssuer : ITokenIssuer
    {
        private readonly AuthOptions _auth;

        public TokenIssuer(IOptions<AuthOptions> auth)
        {
            _auth = auth.Value;
        }
        public string IssueAccessToken()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_auth.SymmetricSecurityKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: _auth.Issuer,
                audience: _auth.Audience,
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return tokenString;
        }
    }
}
