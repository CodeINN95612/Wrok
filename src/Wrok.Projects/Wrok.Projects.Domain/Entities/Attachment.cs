using Wrok.Projects.Domain.ValueObjects;

namespace Wrok.Projects.Domain.Entities;

public sealed class Attachment
{
    public AttachmentId Id { get; private set; }
    public string Name { get; private set; }
    public string Path { get; private set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Attachment other)
        {
            return false;
        }

        return Id.Equals(other.Id) 
            && Name == other.Name
            && Path == other.Path;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Path);
    }
}