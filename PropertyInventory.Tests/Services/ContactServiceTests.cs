using FluentAssertions;
using Moq;
using PropertyInventory.Application.DTOs.Contact;
using PropertyInventory.Application.Interfaces;
using PropertyInventory.Application.Services;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Tests.Services;

public class ContactServiceTests
{
    private readonly Mock<IContactRepository> _repo = new();
    private readonly ContactService _sut;

    public ContactServiceTests()
    {
        _sut = new ContactService(_repo.Object);
    }

    [Fact]
    public async Task GetAllAsync_maps_paged_result()
    {
        var contacts = new List<Contact>
        {
            new() { Id = Guid.NewGuid(), FirstName = "Carmen", LastName = "Attard", EmailAddress = "c@x.com" }
        };
        _repo.Setup(r => r.GetPagedAsync(It.IsAny<ContactFilter>())).ReturnsAsync((contacts, 1));

        var result = await _sut.GetAllAsync(new ContactFilter { Page = 1, PageSize = 10 });

        result.TotalCount.Should().Be(1);
        result.Data.Single().FirstName.Should().Be("Carmen");
    }

    [Fact]
    public async Task GetByIdAsync_includes_owned_properties()
    {
        var id = Guid.NewGuid();
        var contact = new Contact
        {
            Id = id, FirstName = "Joe", LastName = "Borg", EmailAddress = "joe@x.com",
            Ownerships = new List<PropertyOwnership>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    EffectiveFrom = new DateTime(2023, 5, 6),
                    EffectiveTill = null,
                    AcquisitionPrice = 400000m,
                    AcquisitionCurrency = "EUR",
                    Property = new Property { Id = Guid.NewGuid(), Name = "Penthouse", Address = "Valletta" }
                }
            }
        };
        _repo.Setup(r => r.GetByIdAsync(id, true)).ReturnsAsync(contact);

        var result = await _sut.GetByIdAsync(id);

        result!.OwnedProperties.Should().HaveCount(1);
        result.OwnedProperties.Single().PropertyName.Should().Be("Penthouse");
        result.OwnedProperties.Single().IsCurrentOwner.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_returns_null_when_missing()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false)).ReturnsAsync((Contact?)null);

        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UpdateContactDto());

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_only_changes_provided_fields()
    {
        var id = Guid.NewGuid();
        var contact = new Contact { Id = id, FirstName = "Old", LastName = "Name", EmailAddress = "old@x.com", PhoneNumber = "111" };
        _repo.Setup(r => r.GetByIdAsync(id, false)).ReturnsAsync(contact);

        await _sut.UpdateAsync(id, new UpdateContactDto { Id = id, FirstName = "New" });

        contact.FirstName.Should().Be("New");
        contact.LastName.Should().Be("Name");      // unchanged
        contact.EmailAddress.Should().Be("old@x.com"); // unchanged
    }
}
