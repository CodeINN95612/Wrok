namespace Wrok.Identity.Application.Abstractions.UnitOfWork;
internal interface IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default);
}
