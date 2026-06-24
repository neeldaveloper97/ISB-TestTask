namespace PropertyInventory.Domain.Entities;

public class Property
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty; // ISO code e.g. "EUR"
    public DateTime DateOfRegistration { get; set; }

    public ICollection<PropertyOwnership> Ownerships { get; set; } = new List<PropertyOwnership>();
    public ICollection<PropertyPriceHistory> PriceHistories { get; set; } = new List<PropertyPriceHistory>();
}
