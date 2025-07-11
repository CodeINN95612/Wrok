using FluentValidation;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Features.Invitations.Invite;

internal sealed class InviteRequestValidator : AbstractValidator<InviteRequest>
{
    public InviteRequestValidator(IRegexValidator validator)
    {
        var emailRequired = InviteErrors.EmailRequired;
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode(emailRequired.Code)
            .WithMessage(emailRequired.Message);

        var invalidEmail = InviteErrors.InvalidEmail;
        RuleFor(x => x.Email)
            .Must(validator.IsValidEmail)
            .WithErrorCode(invalidEmail.Code)
            .WithMessage(invalidEmail.Message);

        var roleRequired = InviteErrors.RoleRequired;
        RuleFor(x => x.Role)
            .NotEmpty()
            .WithErrorCode(roleRequired.Code)
            .WithMessage(roleRequired.Message);

        var invalidRole = InviteErrors.InvalidRole;
        RuleFor(x => x.Role)
            .Must(IsRoleValid)
            .WithErrorCode(invalidRole.Code)
            .WithMessage(invalidRole.Message);
    }

    private bool IsRoleValid(string role)
    {
        var valid = Enum.TryParse<UserRole>(role, true, out var parsedRole);
        return valid && parsedRole is not UserRole.Freelancer;
    }
}
