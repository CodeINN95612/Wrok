using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Abstractions.Repositories;
public interface IInvitationRepository
{
    public Task<Invitation?> GetByCodeAsync(string code, CancellationToken ct);
}
