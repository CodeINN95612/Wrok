using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Settings;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Extensions;

namespace Wrok.Identity.Application.Common;
internal sealed class JwtGenerator(IOptionsSnapshot<JwtSettings> settings) : IJwtGenerator
{
    public string Generate(User user)
    {
        var jwtSettings = settings.Value;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        ];

        if (user.GetTenant() is Tenant tenant)
        {
            claims.Add(new Claim("tenant", tenant.Id.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
