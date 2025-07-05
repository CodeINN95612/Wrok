namespace Wrok.Identity.Application.Abstractions.Common;
internal interface IRegexValidator
{
    public bool IsValidEmail(string email);
    public bool IsValidPassword(string password);
}
