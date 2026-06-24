using FluentAssertions;
using Moq;
using PropertyInventory.Application.DTOs.Property;
using PropertyInventory.Application.Exceptions;
using PropertyInventory.Application.Interfaces;
using PropertyInventory.Application.Services;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Tests.Services;

public class PropertyServiceTests
{
    private readonly Mock<IPropertyRepository> _repo = new();
    private readonly Mock<IContactRepository> _contacts = new();
    private readonly PropertyService _sut;

    public PropertyServiceTests()
    {
        _sut = new PropertyService(_repo.Object, _contacts.Object);
    }

    [Fact]
    public async Task CreateAsync_creates_initial_price_history_record()
    {
        Property? added = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<Property>()))
             .Callback<Property>(p => { p.Id = Guid.NewGuid(); added = p; })
             .Returns(Task.CompletedTask);
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), true))
             .ReturnsAsync(() => added);

        var dto = new CreatePropertyDto { Name = "Villa", Address = "1 St", Price = 250000m, Currency = "EUR" };

        var result = await _sut.CreateAsync(dto);

        result.Name.Should().Be("Villa");
        _repo.Verify(r => r.AddPriceHistoryAsync(It.Is<PropertyPriceHistory>(
            h => h.Amount == 250000m && h.Currency == "EUR")), Times.Once);
        // Saved twice: once to populate the store-generated Id, once for the price-history row.
        _repo.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task CreateAsync_defaults_registration_date_to_today_when_omitted()
    {
        Property? added = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<Property>()))
             .Callback<Property>(p => { p.Id = Guid.NewGuid(); added = p; })
             .Returns(Task.CompletedTask);
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), true)).ReturnsAsync(() => added);

        await _sut.CreateAsync(new CreatePropertyDto { Name = "X", Address = "Y", Price = 1, Currency = "USD" });

        added!.DateOfRegistration.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public async Task UpdateAsync_adds_price_history_when_price_changes()
    {
        var existing = new Property { Id = Guid.NewGuid(), Name = "A", Address = "B", Price = 100m, Currency = "EUR" };
        _repo.Setup(r => r.GetByIdAsync(existing.Id, true)).ReturnsAsync(existing);

        await _sut.UpdateAsync(existing.Id, new UpdatePropertyDto { Id = existing.Id, Price = 150m });

        existing.Price.Should().Be(150m);
        _repo.Verify(r => r.AddPriceHistoryAsync(It.Is<PropertyPriceHistory>(h => h.Amount == 150m)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_does_not_add_price_history_when_price_unchanged()
    {
        var existing = new Property { Id = Guid.NewGuid(), Name = "A", Address = "B", Price = 100m, Currency = "EUR" };
        _repo.Setup(r => r.GetByIdAsync(existing.Id, true)).ReturnsAsync(existing);

        await _sut.UpdateAsync(existing.Id, new UpdatePropertyDto { Id = existing.Id, Name = "A2", Price = 100m });

        _repo.Verify(r => r.AddPriceHistoryAsync(It.IsAny<PropertyPriceHistory>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_returns_null_when_property_missing()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), true)).ReturnsAsync((Property?)null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UpdatePropertyDto());

        result.Should().BeNull();
    }

    [Fact]
    public async Task AssignOwnerAsync_closes_previous_ownership_and_creates_new()
    {
        var propertyId = Guid.NewGuid();
        var property = new Property { Id = propertyId, Name = "P", Address = "A", Price = 1, Currency = "EUR" };
        var current = new PropertyOwnership { Id = Guid.NewGuid(), PropertyId = propertyId, EffectiveTill = null };

        _repo.Setup(r => r.GetByIdAsync(propertyId, false)).ReturnsAsync(property);
        _repo.Setup(r => r.GetByIdAsync(propertyId, true)).ReturnsAsync(property);
        _repo.Setup(r => r.GetCurrentOwnershipAsync(propertyId)).ReturnsAsync(current);
        _contacts.Setup(c => c.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        var dto = new AssignOwnerDto
        {
            ContactId = Guid.NewGuid(),
            EffectiveFrom = new DateTime(2024, 6, 1),
            AcquisitionPrice = 500000m,
            AcquisitionCurrency = "EUR"
        };

        await _sut.AssignOwnerAsync(propertyId, dto);

        current.EffectiveTill.Should().Be(new DateTime(2024, 6, 1));
        _repo.Verify(r => r.AddOwnershipAsync(It.Is<PropertyOwnership>(
            o => o.ContactId == dto.ContactId && o.EffectiveTill == null && o.AcquisitionPrice == 500000m)), Times.Once);
    }

    [Fact]
    public async Task AssignOwnerAsync_throws_NotFound_when_contact_missing()
    {
        var propertyId = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(propertyId, false))
             .ReturnsAsync(new Property { Id = propertyId, Name = "P", Address = "A", Currency = "EUR" });
        _contacts.Setup(c => c.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        var act = () => _sut.AssignOwnerAsync(propertyId, new AssignOwnerDto { ContactId = Guid.NewGuid(), AcquisitionCurrency = "EUR" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task AssignOwnerAsync_returns_null_when_property_missing()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false)).ReturnsAsync((Property?)null);

        var result = await _sut.AssignOwnerAsync(Guid.NewGuid(), new AssignOwnerDto { AcquisitionCurrency = "EUR" });

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_returns_false_when_missing()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false)).ReturnsAsync((Property?)null);

        (await _sut.DeleteAsync(Guid.NewGuid())).Should().BeFalse();
    }
}
