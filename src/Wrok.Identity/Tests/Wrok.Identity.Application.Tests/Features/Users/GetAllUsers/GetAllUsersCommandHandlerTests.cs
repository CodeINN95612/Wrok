using Moq;

using Wrok.Identity.Application.Abstractions.Providers;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Features.Users.GetAllUsers;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Tests.Features.Users.GetAllUsers;

[TestFixture]
public class GetAllUsersCommandHandlerTests
{
    private Mock<IIdentityProvider> _identityProviderMock = null!;
    private Mock<ITenantRepository> _tenantRepositoryMock = null!;
    private GetAllUsersCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _identityProviderMock = new Mock<IIdentityProvider>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _handler = new GetAllUsersCommandHandler(
            _identityProviderMock.Object,
            _tenantRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenNotAuthenticated()
    {
        var request = new GetAllUsersRequest();
        _identityProviderMock.Setup(p => p.TenantId).Returns((TenantId?)null);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenTenantNotFound()
    {
        var request = new GetAllUsersRequest();
        var tenantId = new TenantId(Guid.NewGuid());
        _identityProviderMock.Setup(p => p.TenantId).Returns(tenantId);
        _tenantRepositoryMock.Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync((Tenant?)null);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task Handle_ShouldReturnUsers_WhenTenantExists()
    {
        var request = new GetAllUsersRequest();
        var tenantId = new TenantId(Guid.NewGuid());
        _identityProviderMock.Setup(p => p.TenantId).Returns(tenantId);
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var pm = new ProjectManagerUser("pm@email.com", "PM", "hash", "salt");
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);
        tenant.AddProjectManagerUser(pm);
        _tenantRepositoryMock.Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync(tenant);

        var result = await _handler.Handle(request, CancellationToken.None);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Value, Is.TypeOf<GetAllUsersResponse>());
            Assert.That(result.Value.Count, Is.EqualTo(2));
            Assert.That(result.Value.Data, Has.Count.EqualTo(2));
            Assert.That(result.Value.Data[0].Email, Is.EqualTo("admin@email.com"));
            Assert.That(result.Value.Data[1].Email, Is.EqualTo("pm@email.com"));
        });
    }
}
