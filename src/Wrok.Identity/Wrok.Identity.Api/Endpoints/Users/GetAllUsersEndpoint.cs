namespace Wrok.Identity.Api.Endpoints.Users;

public static class GetAllUsersEndpoint
{
    public sealed record GetAllUsersResponse(
        Guid Id,
        string Email,
        string FullName,
        string Role,
        Guid? TenantId);

    public static void MapGetAllUsersEndpoint(this IEndpointRouteBuilder app)
    {
        //app.MapGet("/users", async (UserRole? role, IUserService userService, CancellationToken ct) =>
        //{
        //    var result = await userService.GetAllUsersAsync(role, ct);
        //    return result.Match(
        //        users => TypedResults.Ok(users.Select(u => new GetAllUsersResponse(
        //            u.Id.Value,
        //            u.Email,
        //            u.FullName,
        //            u.Role.ToString(),
        //            u.TenantId?.Value))),
        //        errors => CustomResults.ProblemFromErrors(errors));
        //});
    }
}
