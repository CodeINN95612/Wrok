using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Common;
using Wrok.Identity.Application.Extensions;
using Wrok.Identity.Application.Settings;

namespace Wrok.Identity.Application.Tests.Extensions;
[TestFixture]
internal class DependencyInjectionExtensionsTests
{
    [Test]
    public void AddApplication_ShouldRegisterServicesAndSettings()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            [$"{JwtSettings.SectionName}:Issuer"] = "TestIssuer",
            [$"{RefreshTokenSettings.SectionName}:ExpirationDays"] = "5"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();


        // Act
        services.AddApplication(configuration);
        var provider = services.BuildServiceProvider();
        var jwtSettings = provider.GetService<Microsoft.Extensions.Options.IOptions<JwtSettings>>()?.Value;
        var refreshTokenSettings = provider.GetService<Microsoft.Extensions.Options.IOptions<RefreshTokenSettings>>()?.Value;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(provider.GetService<IPasswordHasher>(), Is.InstanceOf<PasswordHasher>());
            Assert.That(provider.GetService<IRegexValidator>(), Is.InstanceOf<RegexValidator>());
            Assert.That(provider.GetService<IJwtGenerator>(), Is.InstanceOf<JwtGenerator>());
            Assert.That(provider.GetService<ITokenGenerator>(), Is.InstanceOf<TokenGenerator>());


            Assert.That(jwtSettings, Is.Not.Null);
            Assert.That(jwtSettings?.Issuer, Is.EqualTo("TestIssuer"));
            Assert.That(refreshTokenSettings, Is.Not.Null);
            Assert.That(refreshTokenSettings?.ExpirationDays, Is.EqualTo(5));
        });
    }
}
