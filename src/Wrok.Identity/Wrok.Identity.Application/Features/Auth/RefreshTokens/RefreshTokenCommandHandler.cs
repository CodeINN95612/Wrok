
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

namespace Wrok.Identity.Application.Features.Auth.RefreshTokens;

internal sealed class RefreshTokenCommandHandler(
    IValidator<RefreshTokenRequest> validator,
    ITokenGenerator refreshTokenGenerator,
    IJwtGenerator jwtGenerator,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IOptions<RefreshTokenSettings> refreshTokenSettings)
    : IRequestHandler<RefreshTokenRequest, ErrorOr<RefreshTokenResponse>>
{
    public async Task<ErrorOr<RefreshTokenResponse>> Handle(RefreshTokenRequest request, CancellationToken ct)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrorOr();
        }

        var user = await userRepository.GetByRefreshTokenAsync(request.Token, ct);
        if (user is null || user.RefreshToken is null)
        {
            return RefreshTokenErrors.RefreshTokenUserNotFound.ToErrorOr(ErrorType.Unauthorized);
        }

        if (user.RefreshToken.IsRevoked)
        {
            return RefreshTokenErrors.RefreshTokenRevoked.ToErrorOr(ErrorType.Unauthorized);
        }

        if (user.RefreshToken.IsExpired)
        {
            return RefreshTokenErrors.RefreshTokenExpired.ToErrorOr(ErrorType.Unauthorized);
        }

        var newJwt = jwtGenerator.Generate(user);
        var newToken = refreshTokenGenerator.Generate();

        user.UpdateRefreshToken(newToken, new RefreshTokenSettingsExpirationPolicy(refreshTokenSettings.Value));

        userRepository.Update(user);

        await unitOfWork.SaveChangesAsync(ct);

        return new RefreshTokenResponse(
            newJwt,
            newToken);
    }
}
