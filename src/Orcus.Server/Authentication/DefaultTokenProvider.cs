using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.Authentication
{
    public class DefaultTokenProvider : IDefaultTokenProvider
    {
        private readonly string _audience;
        private readonly string _issuer;
        private readonly SymmetricSecurityKey _key;
        private readonly SigningCredentials _signingCredentials;
        private readonly TimeSpan _userTokenValidityPeriod;
        private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

        public DefaultTokenProvider(string issuer, string audience, byte[] key, TimeSpan userTokenValidityPeriod)
        {
            _issuer = issuer;
            _audience = audience;
            _userTokenValidityPeriod = userTokenValidityPeriod;
            _key = new SymmetricSecurityKey(key);
            _signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        }

        public JwtSecurityToken GetAccountToken(Account account)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.AccountId.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            return new JwtSecurityToken(_issuer, _audience, claims, null, DateTime.UtcNow.Add(_userTokenValidityPeriod),
                _signingCredentials);
        }

        public TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                IssuerSigningKey = _key,
                ValidAudience = _audience,
                ValidIssuer = _issuer,
                ClockSkew = TimeSpan.Zero // Identity and resource servers are the same.
            };
        }

        public string TokenToString(JwtSecurityToken token)
        {
            return _handler.WriteToken(token);
        }
    }
}