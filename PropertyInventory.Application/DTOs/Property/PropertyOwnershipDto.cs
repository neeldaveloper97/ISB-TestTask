namespace PropertyInventory.Application.DTOs.Property;

public class PropertyOwnershipDto
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public Guid ContactId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTill { get; set; }
    public decimal AcquisitionPrice { get; set; }
    public string AcquisitionCurrency { get; set; } = string.Empty;
    public bool IsCurrentOwner => EffectiveTill == null;
}
