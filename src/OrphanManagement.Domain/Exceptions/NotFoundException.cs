namespace OrphanManagement.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found in the system.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}
