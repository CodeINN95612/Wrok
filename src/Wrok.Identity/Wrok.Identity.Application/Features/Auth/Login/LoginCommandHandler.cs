
using ErrorOr;

using FluentValidation;

using MediatR;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;

namespace Wrok.Identity.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    IValidator<LoginRequest> validator,
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IJwtGenerator jwtGenerator) : IRequestHandler<LoginRequest, ErrorOr<LoginResponse>>
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

        //TODO: Implement refresh token logic

        return new LoginResponse(
            user.Email,
            user.FullName,
            user.Role.ToString(),
            jwt,
            "not-implemented");
    }
}
