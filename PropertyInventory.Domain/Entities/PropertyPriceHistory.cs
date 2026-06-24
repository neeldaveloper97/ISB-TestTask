namespace PropertyInventory.Domain.Entities;

public class PropertyPriceHistory
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }

    public Property Property { get; set; } = null!;
}
