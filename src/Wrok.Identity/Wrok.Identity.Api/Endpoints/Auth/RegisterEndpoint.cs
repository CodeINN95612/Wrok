using Wrok.Identity.Api.Common;
using Wrok.Identity.Application.Abstractions.Services;
using Wrok.Identity.Application.Dtos.Auth;

namespace Wrok.Identity.Api.Endpoints.Auth;

public static class RegisterEndpoint
{
    public sealed record RegisterRequest(string Email, string Password, string FullName, string TenantName);

    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (RegisterRequest request, IAuthService authService) =>
        {
            var result = await authService.RegisterUser(new RegisterUserDto(
                request.Email,
                request.Password,
                request.Email,
                request.TenantName));
            return result.Match(
                userId => TypedResults.Created($"users/{userId.Value}"),
                errors => CustomResults.ProblemFromErrors(errors));
        });
    }
}
