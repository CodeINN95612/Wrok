using FluentValidation;

using Microsoft.Extensions.Options;

using Moq;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Features.Auth.RefreshTokens;
using Wrok.Identity.Application.Policies;
using Wrok.Identity.Application.Settings;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Tests.Features.Auth.RefreshTokens;

[TestFixture]
public class RefreshTokenCommandHandlerTests
{
    private Mock<IValidator<RefreshTokenRequest>> _validatorMock = null!;
    private Mock<ITokenGenerator> _refreshTokenGeneratorMock = null!;
    private Mock<IJwtGenerator> _jwtGeneratorMock = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<IOptions<RefreshTokenSettings>> _refreshTokenSettingsMock = null!;
    private RefreshTokenCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _validatorMock = new Mock<IValidator<RefreshTokenRequest>>();
        _refreshTokenGeneratorMock = new Mock<ITokenGenerator>();
        _jwtGeneratorMock = new Mock<IJwtGenerator>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _refreshTokenSettingsMock = new Mock<IOptions<RefreshTokenSettings>>();
        _refreshTokenSettingsMock.Setup(x => x.Value).Returns(new RefreshTokenSettings { ExpirationDays = 60 });
        _handler = new RefreshTokenCommandHandler(
            _validatorMock.Object,
            _refreshTokenGeneratorMock.Object,
            _jwtGeneratorMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _refreshTokenSettingsMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenValidationFails()
    {
        var request = new RefreshTokenRequest("token");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult(new[] {
            new FluentValidation.Results.ValidationFailure("Token", "Invalid")
        }));

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        var request = new RefreshTokenRequest("token");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(request.Token, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenRefreshTokenRevoked()
    {
        var request = new RefreshTokenRequest("token");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        var user = new AdminUser("email", "name", "hash", "salt", true);
        user.UpdateRefreshToken("token", new Wrok.Identity.Application.Policies.RefreshTokenSettingsExpirationPolicy(_refreshTokenSettingsMock.Object.Value));
        user.RefreshToken!.Revoke();
        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(request.Token, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenRefreshTokenExpired()
    {
        var request = new RefreshTokenRequest("token");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        var user = new AdminUser("email", "name", "hash", "salt", true);
        user.UpdateRefreshToken("token", new RefreshTokenSettingsExpirationPolicy(new RefreshTokenSettings { ExpirationDays = -1 }));
        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(request.Token, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnRefreshTokenResponse_WhenValid()
    {
        var request = new RefreshTokenRequest("token");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        var user = new AdminUser("email", "name", "hash", "salt", true);
        user.UpdateRefreshToken("token", new RefreshTokenSettingsExpirationPolicy(_refreshTokenSettingsMock.Object.Value));
        _userRepositoryMock.Setup(r => r.GetByRefreshTokenAsync(request.Token, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _jwtGeneratorMock.Setup(j => j.Generate(user)).Returns("jwt");
        _refreshTokenGeneratorMock.Setup(r => r.Generate(It.IsAny<int>())).Returns("newtoken");
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.TypeOf<RefreshTokenResponse>());
        Assert.That(result.Value.JwtToken, Is.EqualTo("jwt"));
        Assert.That(result.Value.RefreshToken, Is.EqualTo("newtoken"));
    }
}
