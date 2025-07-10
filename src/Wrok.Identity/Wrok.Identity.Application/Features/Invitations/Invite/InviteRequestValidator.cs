using FluentValidation;

using Wrok.Identity.Application.Abstractions.Common;

namespace Wrok.Identity.Application.Features.Invitations.Invite;

internal sealed class InviteRequestValidator : AbstractValidator<InviteRequest>
{
    public InviteRequestValidator(IRegexValidator validator)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(validator.IsValidEmail);
        RuleFor(x => x.Role)
            .NotEmpty();
    }
}
