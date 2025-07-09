using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Api.Extensions;

public static class EndpointExtensions
{
    public static RouteHandlerBuilder RequireAuthorizedRoles(this RouteHandlerBuilder route, params UserRole[] roles)
    {
        var rolesStr = roles.Select(r => r.ToString()).ToList();
        return route.RequireAuthorization(r => r.RequireRole(rolesStr));
    }
}
