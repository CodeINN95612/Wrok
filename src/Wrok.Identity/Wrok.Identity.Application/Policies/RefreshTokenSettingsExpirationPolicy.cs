
using Wrok.Identity.Application.Settings;
using Wrok.Identity.Domain.Policies;

namespace Wrok.Identity.Application.Policies;
internal sealed class RefreshTokenSettingsExpirationPolicy(
    RefreshTokenSettings settings): IRefreshTokenExpirationPolicy
{
    public DateTime GetExpirationDate()
    {
        return DateTime.UtcNow.AddDays(settings.ExpirationDays);
    }
}
