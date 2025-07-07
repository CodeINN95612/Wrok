namespace Wrok.Identity.Domain.Policies;
public interface IRefreshTokenExpirationPolicy
{
    public DateTime GetExpirationDate();
}
