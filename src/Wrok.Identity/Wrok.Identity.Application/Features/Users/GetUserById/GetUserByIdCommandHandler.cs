
using ErrorOr;

using MediatR;

using Wrok.Identity.Application.Abstractions.Providers;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Dtos.Users;
using Wrok.Identity.Application.Extensions;
using Wrok.Identity.Application.Features.Users.GetUserById;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Features.Users.GetById;

internal sealed class GetUserByIdCommandHandler(
    IIdentityProvider identityProvider,
    ITenantRepository tenantRepository) : IRequestHandler<GetUserByIdRequest, ErrorOr<GetUserByIdResponse>>
{
    public async Task<ErrorOr<GetUserByIdResponse>> Handle(GetUserByIdRequest request, CancellationToken ct)
    {
        var tenantId = identityProvider.TenantId;
        if (tenantId is null)
        {
            return GetUserByIdErrors.NotAuthenticated.ToErrorOr(ErrorType.Unauthorized);
        }

        var tenant = await tenantRepository.GetByIdAsync(tenantId, ct);
        if (tenant is null)
        {
            return GetUserByIdErrors.TenantNotFound.ToErrorOr(ErrorType.NotFound);
        }

        var id = new UserId(request.UserId);
        var user = tenant.GetUser(id);

        if (user is null)
        {
            return GetUserByIdErrors.UserNotFound.ToErrorOr(ErrorType.NotFound);
        }

        var userDto = UserDto.FromUser(user);
        return new GetUserByIdResponse(userDto);
    }
}
