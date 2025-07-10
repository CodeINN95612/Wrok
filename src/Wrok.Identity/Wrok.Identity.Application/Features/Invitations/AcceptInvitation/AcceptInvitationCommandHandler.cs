
using System;

using ErrorOr;

using FluentValidation;

using MediatR;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Features.Invitations.AcceptInvitation;

public sealed record AcceptInvitationRequest(
    string Code,
    string Email,
    string Password,
    string Fullname) : IRequest<ErrorOr<AcceptInvitationResponse>>;

public record struct AcceptInvitationResponse(UserId createdUserId);

internal sealed class AcceptInvitationCommandHandler(
    IValidator<AcceptInvitationRequest> validator,
    ITenantRepository tenantRepository,
    IInvitationRepository invitationRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<AcceptInvitationRequest, ErrorOr<AcceptInvitationResponse>>
{
    public async Task<ErrorOr<AcceptInvitationResponse>> Handle(AcceptInvitationRequest request, CancellationToken ct)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => Error.Validation($"AcceptInvitation.{error.PropertyName}.{error.ErrorCode}", error.ErrorMessage))
                .ToList();
            return errors;
        }

        var invitation = await invitationRepository.GetByCodeAsync(request.Code, ct);
        if (invitation is null)
        {
            return Error.NotFound("AcceptInvitation.NotFound", "Invitation not found.");
        }
        
        if (invitation.AcceptedAt.HasValue)
        {
            return Error.Validation("AcceptInvitation.AlreadyAccepted", "Invitation has already been accepted.");
        }

        var (passwordHash, salt) = passwordHasher.Hash(request.Password);

        User user = invitation.Role switch
        {
            Domain.Enums.UserRole.Admin => new AdminUser(request.Email, request.Fullname, passwordHash, salt),
            Domain.Enums.UserRole.ProjectManager => new ProjectManagerUser(request.Email, request.Fullname, passwordHash, salt),
            _ => throw new InvalidOperationException($"This should never happen")
        };

        invitation.Tenant.JoinByInvite(user, request.Code);
        
        tenantRepository.Update(invitation.Tenant);

        await unitOfWork.SaveChangesAsync(ct);

        return new AcceptInvitationResponse(user.Id);
    }
}

internal sealed class AcceptInvitationRequestValidator : AbstractValidator<AcceptInvitationRequest>
{
    public AcceptInvitationRequestValidator(IRegexValidator regexValidator)
    {
        RuleFor(x => x.Code)
            .NotEmpty();
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(regexValidator.IsValidEmail);
        RuleFor(x => x.Password)
            .NotEmpty()
            .Must(regexValidator.IsValidPassword);
        RuleFor(x => x.Fullname)
            .NotEmpty();
    }
}
