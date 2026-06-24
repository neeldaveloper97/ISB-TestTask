namespace PropertyInventory.Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;

    public ICollection<PropertyOwnership> Ownerships { get; set; } = new List<PropertyOwnership>();
}
