using PropertyInventory.Application.DTOs.Dashboard;
using PropertyInventory.Application.Interfaces;

namespace PropertyInventory.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrencyService _currencyService;

    public DashboardService(IPropertyRepository propertyRepository, ICurrencyService currencyService)
    {
        _propertyRepository = propertyRepository;
        _currencyService = currencyService;
    }

    public async Task<List<DashboardRowDto>> GetDashboardAsync()
    {
        var ownerships = await _propertyRepository.GetAllOwnershipsWithDetailsAsync();

        var rows = new List<DashboardRowDto>();
        foreach (var o in ownerships.OrderByDescending(o => o.EffectiveFrom).ThenByDescending(o => o.Id))
        {
            var usd = await _currencyService.ConvertToUsdAsync(
                o.AcquisitionPrice, o.AcquisitionCurrency, o.EffectiveFrom);

            rows.Add(new DashboardRowDto
            {
                Id = o.Id,
                PropertyId = o.PropertyId,
                PropertyName = o.Property?.Name ?? string.Empty,
                AskingPrice = o.Property?.Price ?? 0m,
                AskingPriceCurrency = o.Property?.Currency ?? string.Empty,
                OwnerName = o.Contact is null ? string.Empty : $"{o.Contact.FirstName} {o.Contact.LastName}",
                DateOfPurchase = o.EffectiveFrom,
                SoldAtPriceOriginal = o.AcquisitionPrice,
                SoldAtPriceCurrency = o.AcquisitionCurrency,
                SoldAtPriceUsd = usd
            });
        }

        return rows;
    }
}
