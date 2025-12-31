namespace OrphanManagement.Domain.Exceptions;

/// <summary>
/// Base exception class for domain-specific exceptions.
/// All custom domain exceptions should inherit from this class.
/// </summary>
public class DomainException : Exception
{
    public DomainException()
    {
    }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
