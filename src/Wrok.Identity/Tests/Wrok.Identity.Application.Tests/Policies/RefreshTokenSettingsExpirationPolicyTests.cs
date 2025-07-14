using Wrok.Identity.Application.Policies;
using Wrok.Identity.Application.Settings;

namespace Wrok.Identity.Application.Tests.Policies;

[TestFixture]
internal class RefreshTokenSettingsExpirationPolicyTests
{
    [TestCase(7)]
    [TestCase(-1)]
    [TestCase(0)]
    public void GetExpirationDate_ShouldReturnDateWithCorrectOffset(int expirationDays)
    {
        var settings = new RefreshTokenSettings { ExpirationDays = expirationDays };
        var policy = new RefreshTokenSettingsExpirationPolicy(settings);

        var before = DateTime.UtcNow;
        var expiration = policy.GetExpirationDate();
        var after = DateTime.UtcNow;

        Assert.Multiple(() =>
        {
            Assert.That(expiration, Is.GreaterThanOrEqualTo(before.AddDays(expirationDays)));
            Assert.That(expiration, Is.LessThanOrEqualTo(after.AddDays(expirationDays)));
        });
    }
}
