namespace PharmacySystem.Domain.Exceptions;

/// <summary>Base exception for all domain rule violations.</summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

/// <summary>Thrown when a requested entity does not exist.</summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"'{entityName}' with key '{key}' was not found.") { }
}

/// <summary>Thrown when a business rule is violated.</summary>
public sealed class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>Thrown when medicine stock is insufficient for a sale.</summary>
public sealed class InsufficientStockException : DomainException
{
    public InsufficientStockException(string medicineName, int requested, int available)
        : base($"Insufficient stock for '{medicineName}'. Requested: {requested}, Available: {available}.") { }
}

/// <summary>Thrown on duplicate data constraint violations.</summary>
public sealed class DuplicateEntryException : DomainException
{
    public DuplicateEntryException(string field, object value)
        : base($"A record with {field} '{value}' already exists.") { }
}

/// <summary>Thrown when an operation is not permitted for the current entity state.</summary>
public sealed class InvalidOperationDomainException : DomainException
{
    public InvalidOperationDomainException(string message) : base(message) { }
}
