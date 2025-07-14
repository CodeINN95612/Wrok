using Wrok.Identity.Application.Errors;

namespace Wrok.Identity.Application.Tests.Errors;
[TestFixture]
internal class CommonErrorsTests
{
    private const string Prefix = "TestPrefix";
    private const string Resource = "TestResource";

    [Test]
    public void PasswordInvalid_ShouldReturnCorrectError()
    {
        var error = CommonErrors.PasswordInvalid(Prefix);

        Assert.Multiple(() =>
        {
            Assert.That(error.Code, Is.EqualTo($"{Prefix}.PasswordInvalid"));
            Assert.That(error.Message, Does.Contain("Password must be at least 8 characters long"));
        });
    }

    [Test]
    public void ResourceNotFound_ShouldReturnCorrectError()
    {
        var error = CommonErrors.ResourceNotFound(Prefix, Resource);

        Assert.Multiple(() =>
        {
            Assert.That(error.Code, Is.EqualTo($"{Prefix}.NotFound"));
            Assert.That(error.Message, Is.EqualTo($"{Resource} not found."));
        });
    }

    [Test]
    public void ResourceRequired_ShouldReturnCorrectError()
    {
        var error = CommonErrors.ResourceRequired(Prefix, Resource);

        Assert.Multiple(() =>
        {
            Assert.That(error.Code, Is.EqualTo($"{Prefix}.Required"));
            Assert.That(error.Message, Is.EqualTo($"{Resource} is mandatory"));
        });
    }

    [Test]
    public void ResourceInvalid_ShouldReturnCorrectError()
    {
        var error = CommonErrors.ResourceInvalid(Prefix, Resource);

        Assert.Multiple(() =>
        {
            Assert.That(error.Code, Is.EqualTo($"{Prefix}.Invalid"));
            Assert.That(error.Message, Is.EqualTo($"{Resource} is invalid."));
        });
    }
}
