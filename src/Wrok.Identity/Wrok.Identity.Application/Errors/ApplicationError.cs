namespace Wrok.Identity.Application.Errors;
internal class ApplicationError(string code, string message)
{
    public string Code => code;
    public string Message => message;

    public override string ToString()
    {
        return $"{Code}: {Message}";
    }
}
