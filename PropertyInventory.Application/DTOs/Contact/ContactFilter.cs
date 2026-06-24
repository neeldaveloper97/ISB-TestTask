namespace PropertyInventory.Application.DTOs.Contact;

public class ContactFilter
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}
