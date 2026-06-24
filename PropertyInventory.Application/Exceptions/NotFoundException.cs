namespace PropertyInventory.Application.Exceptions;

/// <summary>Thrown when a referenced entity does not exist. Mapped to HTTP 404 by the global handler.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public static NotFoundException For(string entity, Guid id) =>
        new($"{entity} with ID {id} was not found.");
}
