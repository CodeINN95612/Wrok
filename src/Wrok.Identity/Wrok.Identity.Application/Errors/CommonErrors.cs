namespace Wrok.Identity.Application.Errors;
internal static class CommonErrors
{
    public static ApplicationError PasswordInvalid(string prefix) => new(
        $"{prefix}.PasswordInvalid",
        "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");

    public static ApplicationError ResourceNotFound(string prefix, string resource) => new(
        $"{prefix}.NotFound",
        $"{resource} not found.");

    public static ApplicationError ResourceRequired(string prefix, string resource) =>
        new($"{prefix}.Required", $"{resource} is mandatory");

    public static ApplicationError ResourceInvalid(string prefix, string resource) =>
        new($"{prefix}.Invalid", $"{resource} is invalid.");
}
