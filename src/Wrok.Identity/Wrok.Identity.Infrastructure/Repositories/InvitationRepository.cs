using Microsoft.EntityFrameworkCore;

using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Infrastructure.Data;

namespace Wrok.Identity.Infrastructure.Repositories;
internal sealed class InvitationRepository(WrokIdentityDbContext db) : IInvitationRepository
{
    public async Task<Invitation?> GetByCodeAsync(string code, CancellationToken ct)
    {
        var invitation = await db.Invitations
            .Where(i => i.Code == code)
            .Include(i => i.Tenant)
            .ThenInclude(t => t.AdminUsers)
            .Include(i => i.Tenant)
            .ThenInclude(t => t.ProjectManagerUsers)
            .FirstOrDefaultAsync(ct);
        return invitation;
    }
}
