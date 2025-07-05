namespace Wrok.Identity.Application.Dtos.Auth;
public sealed record RegisterUserDto(string Email, string Password, string FullName, string TenantName);