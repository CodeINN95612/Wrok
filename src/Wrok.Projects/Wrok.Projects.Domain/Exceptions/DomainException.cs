using Wrok.Shared.Exceptions;

namespace Wrok.Projects.Domain.Exceptions;
public class DomainException : WrokException
{

    private DomainException(string code, string message, Type type) :
        base(code, message, type)
    {
        
    }

    public static void ThrowConflict(string entity, string property, string message)
    {
        throw new DomainException($"Domain.{entity}.Conflict{property}", message, Type.Conflict);
    }
    public static void ThrowNotFound(string entity, string property, string message)
    {
        throw new DomainException($"Domain.{entity}.NotFound{property}", message, Type.Conflict);
    }

    public static void ThrowInvalid(string entity, string property, string message)
    {
        throw new DomainException($"Domain.{entity}.Invalid{property}", message, Type.Validation);
    }

    public static void ThrowIfInvalidId(string entity, string property, Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException($"Domain.{entity}.InvalidId", $"{entity} {property} cannot be an empty Guid.", Type.Validation);
        }
    }

    public static void ThrowIfNullOrWhitespace(string entity, string property, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"Domain.{entity}.Invalid{property}", $"{entity} {property} cannot be null or whitespace.", Type.Validation);
        }
    }
}
