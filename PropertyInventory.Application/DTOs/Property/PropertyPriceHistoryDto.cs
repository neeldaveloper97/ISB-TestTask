namespace PropertyInventory.Application.DTOs.Property;

public class PropertyPriceHistoryDto
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
}
