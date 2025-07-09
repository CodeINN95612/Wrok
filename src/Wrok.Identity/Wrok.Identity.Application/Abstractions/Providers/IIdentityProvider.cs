using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Abstractions.Providers;
public interface IIdentityProvider
{
    public UserId? UserId { get; set; }
    public TenantId? TenantId { get; set; }
}
