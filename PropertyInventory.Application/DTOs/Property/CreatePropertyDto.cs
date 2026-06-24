using System.ComponentModel.DataAnnotations;

namespace PropertyInventory.Application.DTOs.Property;

public class CreatePropertyDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public string Currency { get; set; } = string.Empty;

    public DateTime? DateOfRegistration { get; set; } // defaults to today
}
