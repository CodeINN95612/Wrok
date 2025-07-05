using ErrorOr;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.Services;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Dtos.Auth;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Services;
internal sealed class AuthService(
    ITenantRepository tenantRepository,
    IUnitOfWork unitOfWork,
    IRegexValidator regexValidator,
    IPasswordHasher passwordHasher)
    : IAuthService
{
    public async Task<ErrorOr<UserId>> RegisterUserAsync(RegisterUserDto registerDto, CancellationToken ct)
    {

        var errors = ValidateDto(registerDto).ToList();
        if (errors.Count is > 0)
        {
            return errors;
        }

        var tenant = new Tenant(registerDto.TenantName);

        var (passwordHash, salt) = passwordHasher.Hash(registerDto.Password);
        var superAdminUser = new AdminUser(registerDto.Email, registerDto.FullName, passwordHash, salt, isSuper: true);

        tenant.AddAdminUser(superAdminUser);

        tenantRepository.Add(tenant);
        await unitOfWork.SaveChangesAsync(ct);

        return superAdminUser.Id;
    }

    private IEnumerable<Error> ValidateDto(RegisterUserDto registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Email))
        {
            yield return Error.Validation(
                code: "Auth.Register.EmailRequired",
                description: "Email is required.");
        }
        if (!regexValidator.IsValidEmail(registerDto.Email))
        {
            yield return Error.Validation(
                code: "Auth.Register.InvalidEmail",
                description: "Email is not valid.");
        }
        if (string.IsNullOrWhiteSpace(registerDto.Password))
        {
            yield return Error.Validation(
                code: "Auth.Register.PasswordRequired",
                description: "Password is required.");
        }
        if (!regexValidator.IsValidPassword(registerDto.Password))
        {
            yield return Error.Validation(
                code: "Auth.Register.InvalidPassword",
                description: "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
        }
        if (string.IsNullOrWhiteSpace(registerDto.FullName))
        {
            yield return Error.Validation(
                code: "Auth.Register.FullNameRequired",
                description: "Full name is required.");
        }
        if (string.IsNullOrWhiteSpace(registerDto.TenantName))
        {
            yield return Error.Validation(
                code: "Auth.Register.TenantNameRequired",
                description: "Tenant name is required.");
        }
    }
}
