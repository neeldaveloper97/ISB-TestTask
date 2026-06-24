namespace PropertyInventory.Application.DTOs.Contact;

public class ContactDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>Populated on single-contact fetch: all properties owned past and present.</summary>
    public List<OwnedPropertyDto> OwnedProperties { get; set; } = new();
}
