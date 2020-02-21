using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Velocity.Core.AspNetCore.Services
{
    public class JwtIssuer
    {
        private readonly AuthenticationStateProvider _provider;
        private readonly IConfiguration _configuration;

        public JwtIssuer(AuthenticationStateProvider provider, IConfiguration configuration)
        {
            _provider = provider;
            _configuration = configuration.GetSection("Jwt");
        }

        protected string Token { get; set; }

        public static string GenerateTokenForUser(ClaimsPrincipal user, IConfiguration configuration)
        {
            if (!user.HasClaim(c => c.Type == ClaimTypes.Name)
                || !user.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                return null;

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.FindFirstValue(ClaimTypes.NameIdentifier)),
                new Claim(ClaimTypes.Name, user.FindFirstValue(ClaimTypes.Name)),
            };

            if (user.FindFirst(ClaimTypes.Role) != null)
                claims.Add(new Claim(ClaimTypes.Role, user.FindFirstValue(ClaimTypes.Role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(configuration["JwtExpiryInDays"]));

            var jwt = new JwtSecurityToken(
                configuration["JwtIssuer"],
                configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );


            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        protected virtual string GenerateToken()
        {
            var user = _provider.GetAuthenticationStateAsync()
                .GetAwaiter()
                .GetResult()
                .User;

            Token = GenerateTokenForUser(user, _configuration);

            return Token;
        }


        public virtual string GetToken()
        {
            if (string.IsNullOrWhiteSpace(Token))
                return GenerateToken();

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(Token);

            if (jwt.ValidFrom > DateTime.Now || jwt.ValidTo < DateTime.Now)
                return GenerateToken();

            return Token;
        }
    }
}
