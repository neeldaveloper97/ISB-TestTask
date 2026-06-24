using FluentAssertions;
using Moq;
using PropertyInventory.Application.Interfaces;
using PropertyInventory.Application.Services;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Tests.Services;

public class DashboardServiceTests
{
    private readonly Mock<IPropertyRepository> _repo = new();
    private readonly Mock<ICurrencyService> _currency = new();
    private readonly DashboardService _sut;

    public DashboardServiceTests()
    {
        _sut = new DashboardService(_repo.Object, _currency.Object);
    }

    private static PropertyOwnership Ownership(DateTime from, decimal price, string cur)
    {
        var contact = new Contact { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        var property = new Property { Id = Guid.NewGuid(), Name = "Home", Price = 200000m, Currency = "EUR" };
        return new PropertyOwnership
        {
            Id = Guid.NewGuid(),
            PropertyId = property.Id,
            ContactId = contact.Id,
            EffectiveFrom = from,
            AcquisitionPrice = price,
            AcquisitionCurrency = cur,
            Property = property,
            Contact = contact
        };
    }

    [Fact]
    public async Task GetDashboardAsync_orders_by_effective_from_descending()
    {
        var older = Ownership(new DateTime(2023, 1, 1), 100m, "EUR");
        var newer = Ownership(new DateTime(2024, 1, 1), 200m, "EUR");
        _repo.Setup(r => r.GetAllOwnershipsWithDetailsAsync())
             .ReturnsAsync(new List<PropertyOwnership> { older, newer });
        _currency.Setup(c => c.ConvertToUsdAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                 .ReturnsAsync(1m);

        var rows = await _sut.GetDashboardAsync();

        rows.Should().HaveCount(2);
        rows[0].DateOfPurchase.Should().Be(new DateTime(2024, 1, 1));
        rows[1].DateOfPurchase.Should().Be(new DateTime(2023, 1, 1));
    }

    [Fact]
    public async Task GetDashboardAsync_sets_usd_null_when_conversion_fails()
    {
        var o = Ownership(new DateTime(2024, 1, 1), 120000m, "EUR");
        _repo.Setup(r => r.GetAllOwnershipsWithDetailsAsync())
             .ReturnsAsync(new List<PropertyOwnership> { o });
        _currency.Setup(c => c.ConvertToUsdAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                 .ReturnsAsync((decimal?)null);

        var rows = await _sut.GetDashboardAsync();

        rows.Single().SoldAtPriceUsd.Should().BeNull();
        rows.Single().SoldAtPriceOriginal.Should().Be(120000m);
    }

    [Fact]
    public async Task GetDashboardAsync_maps_converted_usd_value()
    {
        var o = Ownership(new DateTime(2024, 1, 15), 120000m, "EUR");
        _repo.Setup(r => r.GetAllOwnershipsWithDetailsAsync())
             .ReturnsAsync(new List<PropertyOwnership> { o });
        _currency.Setup(c => c.ConvertToUsdAsync(120000m, "EUR", new DateTime(2024, 1, 15)))
                 .ReturnsAsync(130480m);

        var rows = await _sut.GetDashboardAsync();

        rows.Single().SoldAtPriceUsd.Should().Be(130480m);
        rows.Single().OwnerName.Should().Be("John Doe");
    }
}
