using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using FluentValidation;
using Moq;
using NUnit.Framework;
using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Features.Auth.Register;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Tests.Features.Auth.Register;

[TestFixture]
public class RegisterCommandHandlerTests
{
    private Mock<IValidator<RegisterRequest>> _validatorMock = null!;
    private Mock<IPasswordHasher> _passwordHasherMock = null!;
    private Mock<ITenantRepository> _tenantRepositoryMock = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private RegisterCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _validatorMock = new Mock<IValidator<RegisterRequest>>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new RegisterCommandHandler(
            _validatorMock.Object,
            _passwordHasherMock.Object,
            _tenantRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenValidationFails()
    {
        var request = new RegisterRequest("email", "password", "name", "tenant");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult(new[] {
            new FluentValidation.Results.ValidationFailure("Email", "Invalid")
        }));

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenEmailAlreadyInUse()
    {
        var request = new RegisterRequest("email", "password", "name", "tenant");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        _userRepositoryMock.Setup(r => r.IsUniqueByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnRegisterResponse_WhenValid()
    {
        var request = new RegisterRequest("email", "password", "name", "tenant");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        _userRepositoryMock.Setup(r => r.IsUniqueByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _passwordHasherMock.Setup(h => h.Hash(request.Password)).Returns(("hash", "salt"));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.TypeOf<RegisterResponse>());
        Assert.That(result.Value.UserId, Is.Not.EqualTo(default));
    }
}
