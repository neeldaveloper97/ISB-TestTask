namespace PropertyInventory.Application.DTOs.Property;

public class PropertyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime DateOfRegistration { get; set; }
    public List<PropertyOwnershipDto> Ownerships { get; set; } = new();
    public List<PropertyPriceHistoryDto> PriceHistories { get; set; } = new();
}
