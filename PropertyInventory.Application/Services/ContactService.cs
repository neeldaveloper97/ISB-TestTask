using PropertyInventory.Application.DTOs.Common;
using PropertyInventory.Application.DTOs.Contact;
using PropertyInventory.Application.Interfaces;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Application.Services;

public class ContactService : IContactService
{
    private readonly IContactRepository _repository;

    public ContactService(IContactRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ContactDto>> GetAllAsync(ContactFilter filter)
    {
        var (items, total) = await _repository.GetPagedAsync(filter);
        return new PagedResult<ContactDto>
        {
            Data = items.Select(c => MapToDto(c, includeOwned: false)).ToList(),
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ContactDto?> GetByIdAsync(Guid id)
    {
        var contact = await _repository.GetByIdAsync(id, includeOwnerships: true);
        return contact is null ? null : MapToDto(contact, includeOwned: true);
    }

    public async Task<ContactDto> CreateAsync(CreateContactDto dto)
    {
        var contact = new Contact
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            EmailAddress = dto.EmailAddress
        };

        await _repository.AddAsync(contact);
        await _repository.SaveChangesAsync();
        return MapToDto(contact, includeOwned: false);
    }

    public async Task<List<ContactDto>> CreateBulkAsync(IEnumerable<CreateContactDto> dtos)
    {
        var result = new List<ContactDto>();
        foreach (var dto in dtos)
        {
            result.Add(await CreateAsync(dto));
        }
        return result;
    }

    public async Task<ContactDto?> UpdateAsync(Guid id, UpdateContactDto dto)
    {
        var contact = await _repository.GetByIdAsync(id);
        if (contact is null) return null;

        if (dto.FirstName is not null) contact.FirstName = dto.FirstName;
        if (dto.LastName is not null) contact.LastName = dto.LastName;
        if (dto.PhoneNumber is not null) contact.PhoneNumber = dto.PhoneNumber;
        if (dto.EmailAddress is not null) contact.EmailAddress = dto.EmailAddress;

        _repository.Update(contact);
        await _repository.SaveChangesAsync();
        return MapToDto(contact, includeOwned: false);
    }

    public async Task<List<ContactDto>> UpdateBulkAsync(IEnumerable<UpdateContactDto> dtos)
    {
        var result = new List<ContactDto>();
        foreach (var dto in dtos)
        {
            var updated = await UpdateAsync(dto.Id, dto);
            if (updated is not null) result.Add(updated);
        }
        return result;
    }

    private static ContactDto MapToDto(Contact c, bool includeOwned)
    {
        var dto = new ContactDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            PhoneNumber = c.PhoneNumber,
            EmailAddress = c.EmailAddress
        };

        if (includeOwned)
        {
            dto.OwnedProperties = c.Ownerships
                .OrderByDescending(o => o.EffectiveFrom)
                .Select(o => new OwnedPropertyDto
                {
                    OwnershipId = o.Id,
                    PropertyId = o.PropertyId,
                    PropertyName = o.Property?.Name ?? string.Empty,
                    PropertyAddress = o.Property?.Address ?? string.Empty,
                    EffectiveFrom = o.EffectiveFrom,
                    EffectiveTill = o.EffectiveTill,
                    AcquisitionPrice = o.AcquisitionPrice,
                    AcquisitionCurrency = o.AcquisitionCurrency
                }).ToList();
        }

        return dto;
    }
}
