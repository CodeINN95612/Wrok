using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using FluentValidation;
using Moq;
using NUnit.Framework;
using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Providers;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Features.Invitations.Invite;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Tests.Features.Invitations.Invite;

[TestFixture]
public class InviteCommandHandlerTests
{
    private Mock<IValidator<InviteRequest>> _validatorMock = null!;
    private Mock<IIdentityProvider> _identityProviderMock = null!;
    private Mock<ITokenGenerator> _tokenGeneratorMock = null!;
    private Mock<ITenantRepository> _tenantRepositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private InviteCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _validatorMock = new Mock<IValidator<InviteRequest>>();
        _identityProviderMock = new Mock<IIdentityProvider>();
        _tokenGeneratorMock = new Mock<ITokenGenerator>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new InviteCommandHandler(
            _validatorMock.Object,
            _identityProviderMock.Object,
            _tokenGeneratorMock.Object,
            _tenantRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenValidationFails()
    {
        var request = new InviteRequest("email", "Admin");
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult(new[] {
            new FluentValidation.Results.ValidationFailure("Email", "Invalid")
        }));

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenTenantNotFound()
    {
        var request = new InviteRequest("email", "Admin");
        var tenantId = new TenantId(System.Guid.NewGuid());
        var userId = new UserId(System.Guid.NewGuid());
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        _identityProviderMock.Setup(p => p.TenantId).Returns(tenantId);
        _identityProviderMock.Setup(p => p.UserId).Returns(userId);
        _tenantRepositoryMock.Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync((Tenant?)null);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        var request = new InviteRequest("email", "Admin");
        var tenantId = new TenantId(System.Guid.NewGuid());
        var userId = new UserId(System.Guid.NewGuid());
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        _identityProviderMock.Setup(p => p.TenantId).Returns(tenantId);
        _identityProviderMock.Setup(p => p.UserId).Returns(userId);
        var tenant = new Tenant("Tenant");
        _tenantRepositoryMock.Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync(tenant);
        // No admin user in tenant

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnInviteResponse_WhenValid()
    {
        var request = new InviteRequest("invite@email.com", "ProjectManager");
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);
        _validatorMock.Setup(v => v.Validate(request)).Returns(new FluentValidation.Results.ValidationResult());
        _identityProviderMock.Setup(p => p.TenantId).Returns(tenant.Id);
        _identityProviderMock.Setup(p => p.UserId).Returns(admin.Id);
        _tenantRepositoryMock.Setup(r => r.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tenant);
        _tokenGeneratorMock.Setup(t => t.Generate(It.IsAny<int>())).Returns("invitecode");
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Value, Is.TypeOf<InviteResponse>());
        });
    }
}
