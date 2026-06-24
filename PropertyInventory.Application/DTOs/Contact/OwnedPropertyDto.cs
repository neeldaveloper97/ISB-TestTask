namespace PropertyInventory.Application.DTOs.Contact;

/// <summary>A property a contact owns or has owned, shown on the contact detail view.</summary>
public class OwnedPropertyDto
{
    public Guid OwnershipId { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTill { get; set; }
    public decimal AcquisitionPrice { get; set; }
    public string AcquisitionCurrency { get; set; } = string.Empty;
    public bool IsCurrentOwner => EffectiveTill == null;
}
