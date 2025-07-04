using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Abstractions.Repositories;

public interface ITenantRepository
{
    public Task<Tenant> GetByIdAsync(TenantId tenantId, CancellationToken ct = default);
}
