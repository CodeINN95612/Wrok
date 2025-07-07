
using ErrorOr;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Options;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Policies;
using Wrok.Identity.Application.Settings;

namespace Wrok.Identity.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    IValidator<LoginRequest> validator,
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtGenerator jwtGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IOptions<RefreshTokenSettings> refreshTokenSettings) : IRequestHandler<LoginRequest, ErrorOr<LoginResponse>>
{
    public async Task<ErrorOr<LoginResponse>> Handle(LoginRequest request, CancellationToken ct)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => Error.Validation($"Login.{error.PropertyName}.{error.ErrorCode}", error.ErrorMessage))
                .ToList();
            return errors;
        }

        var user = await userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null)
        {
            return Error.Validation("Login.InvalidCredentials", "Invalid email or password.");
        }

        var passwordHash = passwordHasher.Hash(request.Password, user.Salt);
        if (user.PasswordHash != passwordHash)
        {
            return Error.Validation("Login.InvalidCredentials", "Invalid email or password.");
        }

        var jwt = jwtGenerator.Generate(user);
        var refreshToken = refreshTokenGenerator.Generate();

        user.UpdateRefreshToken(
            refreshToken, 
            new RefreshTokenSettingsExpirationPolicy(refreshTokenSettings.Value));
        userRepository.Update(user);

        await unitOfWork.SaveChangesAsync(ct);

        return new LoginResponse(
            user.Email,
            user.FullName,
            user.Role.ToString(),
            jwt,
            refreshToken);
    }
}
