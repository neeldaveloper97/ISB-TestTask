namespace PropertyInventory.Application.DTOs.Property;

public class UpdatePropertyDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public decimal? Price { get; set; }   // when changed, triggers a price history entry
    public string? Currency { get; set; }
}
