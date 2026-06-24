using System.ComponentModel.DataAnnotations;

namespace PropertyInventory.Application.DTOs.Property;

public class AssignOwnerDto
{
    [Required]
    public Guid ContactId { get; set; }

    public DateTime? EffectiveFrom { get; set; } // defaults to today

    [Range(0, double.MaxValue)]
    public decimal AcquisitionPrice { get; set; }

    [Required]
    public string AcquisitionCurrency { get; set; } = string.Empty;
}
