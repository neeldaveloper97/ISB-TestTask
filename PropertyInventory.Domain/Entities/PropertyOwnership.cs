namespace PropertyInventory.Domain.Entities;

/// <summary>
/// Join entity tracking who owns/owned a property and when.
/// EffectiveTill == null means the contact is the current owner.
/// </summary>
public class PropertyOwnership
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public Guid ContactId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTill { get; set; } // null = current owner
    public decimal AcquisitionPrice { get; set; }
    public string AcquisitionCurrency { get; set; } = string.Empty;

    public Property Property { get; set; } = null!;
    public Contact Contact { get; set; } = null!;
}
