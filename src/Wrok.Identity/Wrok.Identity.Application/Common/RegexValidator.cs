using System.Text.RegularExpressions;

using Wrok.Identity.Application.Abstractions.Common;

namespace Wrok.Identity.Application.Common;

internal partial class RegexValidator : IRegexValidator
{

    [System.Text.RegularExpressions.GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial System.Text.RegularExpressions.Regex EmailRegex();

    [System.Text.RegularExpressions.GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")]
    private static partial System.Text.RegularExpressions.Regex PasswordRegex();

    public bool IsValidEmail(string email)
    {
        return EmailRegex().IsMatch(email);
    }

    public bool IsValidPassword(string password)
    {
        return PasswordRegex().IsMatch(password);
    }
}
