
using ErrorOr;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Options;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Extensions;
using Wrok.Identity.Application.Policies;
using Wrok.Identity.Application.Settings;

namespace Wrok.Identity.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    IValidator<LoginRequest> validator,
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IJwtGenerator jwtGenerator,
    ITokenGenerator refreshTokenGenerator,
    IOptions<RefreshTokenSettings> refreshTokenSettings) : IRequestHandler<LoginRequest, ErrorOr<LoginResponse>>
{
    public async Task<ErrorOr<LoginResponse>> Handle(LoginRequest request, CancellationToken ct)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrorOr();
        }

        var user = await userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null)
        {
            return LoginErrors.InvalidCredentials.ToErrorOr(ErrorType.Unauthorized);
        }
        
        var passwordHash = passwordHasher.Hash(request.Password, user.Salt);
        if (user.PasswordHash != passwordHash)
        {
            return LoginErrors.InvalidCredentials.ToErrorOr(ErrorType.Unauthorized);
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