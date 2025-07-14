using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.Extensions.Options;

using Moq;

using Wrok.Identity.Application.Common;
using Wrok.Identity.Application.Settings;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Tests.Common;
[TestFixture]
internal class JwtGeneratorTests
{
    private readonly JwtSettings _jwtSettings = new()
    {
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        Secret = "supersecretkesy123456789012345678912345678",
        ExpirationMinutes = 60
    };

    private static JwtGenerator CreateGeneratorWithMockedOptions(JwtSettings settings)
    {
        var optionsMock = new Mock<IOptionsSnapshot<JwtSettings>>();
        optionsMock.Setup(o => o.Value).Returns(settings);
        return new JwtGenerator(optionsMock.Object);
    }

    [Test]
    public void Generate_ShouldReturnValidJwtToken_WithExpectedClaims()
    {
        // Arrange
        var user = new FreelancerUser("user@example.com", "John Doe", "123", "salt", "title", "bio");
        var generator = CreateGeneratorWithMockedOptions(_jwtSettings);

        // Act
        var token = generator.Generate(user);

        // Asserts
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);


        Assert.Multiple(() =>
        {
            Assert.That(jwt.Issuer, Is.EqualTo(_jwtSettings.Issuer));
            Assert.That(jwt.Audiences, Contains.Item(_jwtSettings.Audience));
            Assert.That(jwt.Claims, Has.Some.Matches<Claim>(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.Value.ToString()));
            Assert.That(jwt.Claims, Has.Some.Matches<Claim>(c => c.Type == ClaimTypes.Name && c.Value == user.FullName));
            Assert.That(jwt.Claims, Has.Some.Matches<Claim>(c => c.Type == ClaimTypes.Email && c.Value == user.Email));
            Assert.That(jwt.Claims, Has.Some.Matches<Claim>(c => c.Type == ClaimTypes.Role && c.Value == user.Role.ToString()));
            Assert.That(jwt.ValidTo, Is.EqualTo(DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes)).Within(TimeSpan.FromSeconds(5)));
        });
    }

    [Test]
    public void Generate_ShouldIncludeTenantClaim_WhenUserHasTenant()
    {
        // Arrange
        var user = new AdminUser("user@example.com", "John Doe", "123", "salt", true);
        var tenant = new Tenant("TenantName");
        tenant.AddAdminUser(user);

        var generator = CreateGeneratorWithMockedOptions(_jwtSettings);

        // Act
        var token = generator.Generate(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.That(jwt.Claims, Has.Some.Matches<Claim>(c => c.Type == "tenant" && c.Value == tenant.Id.Value.ToString()));
    }
}
