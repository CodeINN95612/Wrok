using Wrok.Identity.Application.Errors;

namespace Wrok.Identity.Application.Tests.Errors;
[TestFixture]
internal class ApplicationErrorTests
{
    [TestCase("ERR001", "An error occurred.")]
    [TestCase("404", "Not found.")]
    [TestCase("", "")]
    public void Properties_ShouldReturnConstructorValues(string code, string message)
    {
        var error = new ApplicationError(code, message);

        Assert.Multiple(() =>
        {
            Assert.That(error.Code, Is.EqualTo(code));
            Assert.That(error.Message, Is.EqualTo(message));
        });
    }

    [TestCase("ERR001", "An error occurred.", "ERR001: An error occurred.")]
    [TestCase("404", "Not found.", "404: Not found.")]
    [TestCase("", "", ": ")]
    public void ToString_ShouldReturnFormattedString(string code, string message, string expected)
    {
        var error = new ApplicationError(code, message);

        Assert.That(error.ToString(), Is.EqualTo(expected));
    }
}
