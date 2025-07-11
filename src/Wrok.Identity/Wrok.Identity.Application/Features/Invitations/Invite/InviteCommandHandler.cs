
using ErrorOr;

using FluentValidation;

using MediatR;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Providers;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Extensions;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Features.Invitations.Invite;

internal sealed class InviteCommandHandler(
    IValidator<InviteRequest> validator,
    IIdentityProvider identityProvider,
    ITokenGenerator tokenGenerator,
    ITenantRepository tenantRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<InviteRequest, ErrorOr<InviteResponse>>
{
    public async Task<ErrorOr<InviteResponse>> Handle(InviteRequest request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request); 
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrorOr();
        }

        var tenantId = identityProvider.TenantId!;
        var userId = identityProvider.UserId!;

        var tenant = await tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant is null)
        {
            return InviteErrors.TenantNotFound.ToErrorOr(ErrorType.Unauthorized);
        }

        var user = tenant.GetAdminUser(userId);
        if (user is null)
        {
            return InviteErrors.UserNotFound.ToErrorOr(ErrorType.Forbidden);
        }

        UserRole role = Enum.Parse<UserRole>(request.Role, true);
        var generateInviteCode = tokenGenerator.Generate(10);

        var invitation = new Invitation(user, request.Email, role, generateInviteCode);

        tenant.Invite(invitation);
        tenantRepository.Update(tenant);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new InviteResponse(invitation.Id);
    }
}