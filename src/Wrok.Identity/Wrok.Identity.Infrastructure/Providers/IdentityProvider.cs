using Wrok.Identity.Application.Abstractions.Providers;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Infrastructure.Providers;
internal class IdentityProvider : IIdentityProvider
{
    public UserId? UserId { get; set; }
    public TenantId? TenantId { get; set; }
}
