namespace PropertyInventory.Application.DTOs.Dashboard;

public class DashboardRowDto
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public decimal AskingPrice { get; set; }
    public string AskingPriceCurrency { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public DateTime DateOfPurchase { get; set; }
    public decimal SoldAtPriceOriginal { get; set; }
    public string SoldAtPriceCurrency { get; set; } = string.Empty;
    public decimal? SoldAtPriceUsd { get; set; } // null if conversion failed
}
