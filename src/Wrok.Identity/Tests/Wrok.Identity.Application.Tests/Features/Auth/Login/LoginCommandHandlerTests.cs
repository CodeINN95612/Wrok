using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using FluentValidation;
using Moq;
using NUnit.Framework;
using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Features.Auth.Login;
using Wrok.Identity.Application.Policies;
using Wrok.Identity.Application.Settings;
using Microsoft.Extensions.Options;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Tests.Features.Auth.Login;

[TestFixture]
public class LoginCommandHandlerTests
{
    private Mock<IValidator<LoginRequest>> _validatorMock = null!;
    private Mock<IPasswordHasher> _passwordHasherMock = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<IJwtGenerator> _jwtGeneratorMock = null!;
    private Mock<ITokenGenerator> _refreshTokenGeneratorMock = null!;
    private Mock<IOptions<RefreshTokenSettings>> _refreshTokenSettingsMock = null!;
    private LoginCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _validatorMock = new Mock<IValidator<LoginRequest>>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _jwtGeneratorMock = new Mock<IJwtGenerator>();
        _refreshTokenGeneratorMock = new Mock<ITokenGenerator>();
        _refreshTokenSettingsMock = new Mock<IOptions<RefreshTokenSettings>>();
        _refreshTokenSettingsMock.Setup(x => x.Value).Returns(new RefreshTokenSettings { ExpirationDays = 60 });
        _handler = new LoginCommandHandler(
            _validatorMock.Object,
            _passwordHasherMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _jwtGeneratorMock.Object,
            _refreshTokenGeneratorMock.Object,
            _refreshTokenSettingsMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenValidationFails()
    {
        var request = new LoginRequest("email", "password");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult(new[] {
            new FluentValidation.Results.ValidationFailure("Email", "Invalid")
        }));

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        var request = new LoginRequest("email", "password");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenPasswordDoesNotMatch()
    {
        var request = new LoginRequest("email", "password");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        var user = new AdminUser("email", "name", "hash", "salt", true);
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Hash(request.Password, user.Salt)).Returns("wronghash");

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnLoginResponse_WhenCredentialsAreValid()
    {
        var request = new LoginRequest("email", "password");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        var user = new AdminUser("email", "name", "hash", "salt", true);
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Hash(request.Password, user.Salt)).Returns("hash");
        _jwtGeneratorMock.Setup(j => j.Generate(user)).Returns("jwt");
        _refreshTokenGeneratorMock.Setup(r => r.Generate(It.IsAny<int>())).Returns("refresh");
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Value, Is.TypeOf<LoginResponse>());
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.Value.Email, Is.EqualTo("email"));
            Assert.That(result.Value.Token, Is.EqualTo("jwt"));
            Assert.That(result.Value.RefreshToken, Is.EqualTo("refresh"));
        });
    }
}
