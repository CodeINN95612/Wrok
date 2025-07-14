using Wrok.Identity.Application.Common;

namespace Wrok.Identity.Application.Tests.Common;
[TestFixture]
internal class RegexValidatorTests
{
    private RegexValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new RegexValidator();
    }

    [TestCase("user@example.com", true)]
    [TestCase("user.name+tag@sub.domain.co", true)]
    [TestCase("user@localhost", false)]
    [TestCase("user@", false)]
    [TestCase("@example.com", false)]
    [TestCase("userexample.com", false)]
    [TestCase("", false)]
    [TestCase("user@.com", false)]
    public void IsValidEmail_ShouldValidateEmailCorrectly(string email, bool expected)
    {
        var result = _validator.IsValidEmail(email);
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("Password1!", true)]
    [TestCase("P@ssw0rd", true)]
    [TestCase("passw0rd!", false)] // missing uppercase
    [TestCase("PASSWORD1!", false)] // missing lowercase
    [TestCase("Password!", false)] // missing digit
    [TestCase("Password1", false)] // missing special char
    [TestCase("P1!", false)] // too short
    [TestCase("", false)]
    public void IsValidPassword_ShouldValidatePasswordCorrectly(string password, bool expected)
    {
        var result = _validator.IsValidPassword(password);
        Assert.That(result, Is.EqualTo(expected));
    }
}
