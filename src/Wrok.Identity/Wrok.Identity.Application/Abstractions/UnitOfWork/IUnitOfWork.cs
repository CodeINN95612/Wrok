namespace Wrok.Identity.Application.Abstractions.UnitOfWork;
public interface IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default);
}
