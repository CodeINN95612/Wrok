using FluentValidation;

using Moq;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Common;
using Wrok.Identity.Application.Features.Invitations.AcceptInvitation;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Tests.Features.Invitations.AcceptInvitation;

[TestFixture]
public class AcceptInvitationCommandHandlerTests
{
    private Mock<IValidator<AcceptInvitationRequest>> _validatorMock = null!;
    private Mock<ITenantRepository> _tenantRepositoryMock = null!;
    private Mock<IInvitationRepository> _invitationRepositoryMock = null!;
    private Mock<IPasswordHasher> _passwordHasherMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private AcceptInvitationCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _validatorMock = new Mock<IValidator<AcceptInvitationRequest>>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _invitationRepositoryMock = new Mock<IInvitationRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AcceptInvitationCommandHandler(
            _validatorMock.Object,
            _tenantRepositoryMock.Object,
            _invitationRepositoryMock.Object,
            _passwordHasherMock.Object,
            _unitOfWorkMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenValidationFails()
    {
        var request = new AcceptInvitationRequest("code", "email", "fullname", "password");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult(new[] {
            new FluentValidation.Results.ValidationFailure("Code", "Invalid")
        }));

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenInvitationNotFound()
    {
        var request = new AcceptInvitationRequest("code", "email", "fullname", "password");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        _invitationRepositoryMock.Setup(r => r.GetByCodeAsync(request.Code, It.IsAny<CancellationToken>())).ReturnsAsync((Invitation?)null);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenInvitationAlreadyAccepted()
    {
        var request = new AcceptInvitationRequest("code", "email", "fullname", "password");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);
        var invitation = new Invitation(admin, "invite@email.com", UserRole.ProjectManager, "code");
        tenant.Invite(invitation);
        var newUser = new ProjectManagerUser(request.Email, request.Fullname, "passwordHash", "salt");
        tenant.JoinByInvite(newUser, request.Code);
        _invitationRepositoryMock.Setup(r => r.GetByCodeAsync(request.Code, It.IsAny<CancellationToken>())).ReturnsAsync(invitation);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnAcceptInvitationResponse_WhenValid()
    {
        var request = new AcceptInvitationRequest("code", "pm@email.com", "PM", "password");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);
        var invitation = new Invitation(admin, "pm@email.com", UserRole.ProjectManager, "code");
        tenant.Invite(invitation);
        _invitationRepositoryMock.Setup(r => r.GetByCodeAsync(request.Code, It.IsAny<CancellationToken>())).ReturnsAsync(invitation);
        _passwordHasherMock.Setup(h => h.Hash(request.Password)).Returns(("hash", "salt"));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.TypeOf<AcceptInvitationResponse>());
    }
}
