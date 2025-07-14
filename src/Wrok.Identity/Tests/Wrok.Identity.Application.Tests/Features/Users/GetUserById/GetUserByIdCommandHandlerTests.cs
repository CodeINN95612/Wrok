using Moq;

using Wrok.Identity.Application.Abstractions.Providers;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Features.Users.GetById;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Tests.Features.Users.GetUserById;

[TestFixture]
public class GetUserByIdCommandHandlerTests
{
    private Mock<IIdentityProvider> _identityProviderMock = null!;
    private Mock<ITenantRepository> _tenantRepositoryMock = null!;
    private GetUserByIdCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _identityProviderMock = new Mock<IIdentityProvider>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _handler = new GetUserByIdCommandHandler(
            _identityProviderMock.Object,
            _tenantRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenNotAuthenticated()
    {
        var request = new GetUserByIdRequest(Guid.NewGuid());
        _identityProviderMock.Setup(p => p.TenantId).Returns((TenantId?)null);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenTenantNotFound()
    {
        var request = new GetUserByIdRequest(Guid.NewGuid());
        _identityProviderMock.Setup(p => p.TenantId).Returns(new TenantId(Guid.NewGuid()));
        _tenantRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<TenantId>(), It.IsAny<CancellationToken>())).ReturnsAsync((Tenant?)null);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        var request = new GetUserByIdRequest(Guid.NewGuid());
        var tenantId = new TenantId(Guid.NewGuid());
        _identityProviderMock.Setup(p => p.TenantId).Returns(tenantId);
        var tenant = new Tenant("Tenant");
        _tenantRepositoryMock.Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync(tenant);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);
        var request = new GetUserByIdRequest(admin.Id.Value);
        _identityProviderMock.Setup(p => p.TenantId).Returns(tenant.Id);
        _tenantRepositoryMock.Setup(r => r.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tenant);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Value, Is.TypeOf<GetUserByIdResponse>());
            Assert.That(result.Value.User.Email, Is.EqualTo(admin.Email));
        });
    }
}
