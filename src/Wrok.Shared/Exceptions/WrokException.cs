namespace Wrok.Shared.Exceptions;

public abstract class WrokException : Exception
{
    public enum Type
    {
        Conflict,
        Validation,
        NotFound,
        Unexpected
    };

    public string ErrorCode { get; private set; }
    public Type ErrorType { get; private set; }
    protected WrokException(string errorCode, string errorMessage, Type type, Exception innerException) :
        base(errorMessage, innerException)
    {
        ErrorCode = errorCode;
        ErrorType = type;
    }
    protected WrokException(string errorCode, string errorMessage, Type type) : 
        base(errorMessage)
    {
        ErrorCode = errorCode;
        ErrorType = type;
    }

    public override string ToString()
    {
        return $"Error Code: {ErrorCode}. " +  base.ToString();
    }
}
