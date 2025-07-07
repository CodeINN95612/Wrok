namespace Wrok.Identity.Application.Settings;
public sealed class RefreshTokenSettings
{
    public const string SectionName = "RefreshToken";
    public required int ExpirationDays { get; init; }
}
