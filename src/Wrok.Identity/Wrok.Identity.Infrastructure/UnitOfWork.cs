using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Infrastructure.Data;

namespace Wrok.Identity.Infrastructure;
internal sealed class UnitOfWork(WrokIdentityDbContext db) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await db.SaveChangesAsync(ct);
    }
}
