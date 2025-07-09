
using Microsoft.EntityFrameworkCore;

using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Infrastructure.Data;

namespace Wrok.Identity.Infrastructure.Repositories;

internal sealed class TenantRepository(WrokIdentityDbContext db) : ITenantRepository
{
    public void Add(Tenant tenant)
    {
        db.Tenants.Add(tenant);
    }

    public void Delete(Tenant tenant)
    {
        db.Tenants.Remove(tenant);
    }

    public async Task<Tenant?> GetByIdAsync(TenantId tenantId, CancellationToken ct = default)
    {
        return await db.Tenants
            .Include(t => t.AdminUsers)
            .Include(t => t.ProjectManagerUsers)
            .Include(t => t.Invitations)
            .FirstOrDefaultAsync(t => t.Id == tenantId, ct);
    }

    public void Update(Tenant tenant)
    {
        db.Tenants.Update(tenant);
    }
}
