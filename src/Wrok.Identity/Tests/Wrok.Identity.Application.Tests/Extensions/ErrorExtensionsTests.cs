using ErrorOr;

using FluentValidation.Results;

using Wrok.Identity.Application.Errors;
using Wrok.Identity.Application.Extensions;

namespace Wrok.Identity.Application.Tests.Extensions;
[TestFixture]
internal class ErrorExtensionsTests
{
    [Test]
    public void ToErrorOr_WithValidationFailures_ReturnsValidationErrors()
    {
        var failures = new List<ValidationFailure>
        {
            new("Prop1", "Message1") { ErrorCode = "Code1" },
            new("Prop2", "Message2") { ErrorCode = "Code2" }
        };

        var errors = failures.ToErrorOr();

        Assert.Multiple(() =>
        {
            Assert.That(errors, Has.Count.EqualTo(2));
            Assert.That(errors[0].Type, Is.EqualTo(ErrorType.Validation));
            Assert.That(errors[0].Code, Is.EqualTo("Code1"));
            Assert.That(errors[0].Description, Is.EqualTo("Message1"));
            Assert.That(errors[1].Type, Is.EqualTo(ErrorType.Validation));
            Assert.That(errors[1].Code, Is.EqualTo("Code2"));
            Assert.That(errors[1].Description, Is.EqualTo("Message2"));
        });
    }

    [TestCase(ErrorType.NotFound)]
    [TestCase(ErrorType.Failure)]
    [TestCase(ErrorType.Unexpected)]
    public void ToErrorOr_WithApplicationError_ReturnsCustomError(ErrorType type)
    {
        var appError = new ApplicationError("SomeCode", "SomeMessage");

        var error = appError.ToErrorOr(type);

        Assert.Multiple(() =>
        {
            Assert.That(error.Type, Is.EqualTo(type));
            Assert.That(error.Code, Is.EqualTo("SomeCode"));
            Assert.That(error.Description, Is.EqualTo("SomeMessage"));
            Assert.That(error.NumericType, Is.EqualTo((int)type));
        });
    }
}
    