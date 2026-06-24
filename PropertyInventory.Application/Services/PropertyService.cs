using PropertyInventory.Application.DTOs.Common;
using PropertyInventory.Application.DTOs.Property;
using PropertyInventory.Application.Exceptions;
using PropertyInventory.Application.Interfaces;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _repository;
    private readonly IContactRepository _contactRepository;

    public PropertyService(IPropertyRepository repository, IContactRepository contactRepository)
    {
        _repository = repository;
        _contactRepository = contactRepository;
    }

    public async Task<PagedResult<PropertyDto>> GetAllAsync(PropertyFilter filter)
    {
        var (items, total) = await _repository.GetPagedAsync(filter);
        return new PagedResult<PropertyDto>
        {
            Data = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<PropertyDto?> GetByIdAsync(Guid id)
    {
        var property = await _repository.GetByIdAsync(id, includeDetails: true);
        return property is null ? null : MapToDto(property);
    }

    public async Task<PropertyDto> CreateAsync(CreatePropertyDto dto)
    {
        var property = new Property
        {
            Name = dto.Name,
            Address = dto.Address,
            Price = dto.Price,
            Currency = dto.Currency,
            DateOfRegistration = dto.DateOfRegistration ?? DateTime.UtcNow.Date
        };

        // Persist first so the store-generated Id (NEWID()) is populated before we
        // reference it as the price-history foreign key.
        await _repository.AddAsync(property);
        await _repository.SaveChangesAsync();

        // Initial price history record reflecting the starting price.
        await _repository.AddPriceHistoryAsync(new PropertyPriceHistory
        {
            PropertyId = property.Id,
            Amount = property.Price,
            Currency = property.Currency,
            EffectiveDate = property.DateOfRegistration
        });

        await _repository.SaveChangesAsync();

        // Re-fetch with details so the response includes the generated price-history row.
        var created = await _repository.GetByIdAsync(property.Id, includeDetails: true);
        return MapToDto(created!);
    }

    public async Task<List<PropertyDto>> CreateBulkAsync(IEnumerable<CreatePropertyDto> dtos)
    {
        var result = new List<PropertyDto>();
        foreach (var dto in dtos)
        {
            result.Add(await CreateAsync(dto));
        }
        return result;
    }

    public async Task<PropertyDto?> UpdateAsync(Guid id, UpdatePropertyDto dto)
    {
        var property = await _repository.GetByIdAsync(id, includeDetails: true);
        if (property is null) return null;

        if (dto.Name is not null) property.Name = dto.Name;
        if (dto.Address is not null) property.Address = dto.Address;
        if (dto.Currency is not null) property.Currency = dto.Currency;

        // A price change adds a new price-history record; the old record is preserved.
        if (dto.Price.HasValue && dto.Price.Value != property.Price)
        {
            property.Price = dto.Price.Value;
            await _repository.AddPriceHistoryAsync(new PropertyPriceHistory
            {
                PropertyId = property.Id,
                Amount = dto.Price.Value,
                Currency = property.Currency,
                EffectiveDate = DateTime.UtcNow.Date
            });
        }

        _repository.Update(property);
        await _repository.SaveChangesAsync();

        var updated = await _repository.GetByIdAsync(property.Id, includeDetails: true);
        return MapToDto(updated!);
    }

    public async Task<List<PropertyDto>> UpdateBulkAsync(IEnumerable<UpdatePropertyDto> dtos)
    {
        var result = new List<PropertyDto>();
        foreach (var dto in dtos)
        {
            var updated = await UpdateAsync(dto.Id, dto);
            if (updated is not null) result.Add(updated);
        }
        return result;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var property = await _repository.GetByIdAsync(id);
        if (property is null) return false;

        _repository.Remove(property);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<PropertyDto?> AssignOwnerAsync(Guid propertyId, AssignOwnerDto dto)
    {
        var property = await _repository.GetByIdAsync(propertyId);
        if (property is null) return null;

        if (!await _contactRepository.ExistsAsync(dto.ContactId))
            throw NotFoundException.For("Contact", dto.ContactId);

        var effectiveFrom = dto.EffectiveFrom ?? DateTime.UtcNow.Date;

        // Close the current ownership (if any) the day the new owner takes over.
        var current = await _repository.GetCurrentOwnershipAsync(propertyId);
        if (current is not null)
        {
            current.EffectiveTill = effectiveFrom;
            _repository.Update(property); // ensure context tracks the change
        }

        await _repository.AddOwnershipAsync(new PropertyOwnership
        {
            PropertyId = propertyId,
            ContactId = dto.ContactId,
            EffectiveFrom = effectiveFrom,
            EffectiveTill = null,
            AcquisitionPrice = dto.AcquisitionPrice,
            AcquisitionCurrency = dto.AcquisitionCurrency
        });

        await _repository.SaveChangesAsync();

        var refreshed = await _repository.GetByIdAsync(propertyId, includeDetails: true);
        return MapToDto(refreshed!);
    }

    private static PropertyDto MapToDto(Property p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Address = p.Address,
        Price = p.Price,
        Currency = p.Currency,
        DateOfRegistration = p.DateOfRegistration,
        Ownerships = p.Ownerships
            .OrderByDescending(o => o.EffectiveFrom)
            .Select(o => new PropertyOwnershipDto
            {
                Id = o.Id,
                PropertyId = o.PropertyId,
                ContactId = o.ContactId,
                ContactName = o.Contact is null ? string.Empty : $"{o.Contact.FirstName} {o.Contact.LastName}",
                EffectiveFrom = o.EffectiveFrom,
                EffectiveTill = o.EffectiveTill,
                AcquisitionPrice = o.AcquisitionPrice,
                AcquisitionCurrency = o.AcquisitionCurrency
            }).ToList(),
        PriceHistories = p.PriceHistories
            .OrderByDescending(h => h.EffectiveDate)
            .Select(h => new PropertyPriceHistoryDto
            {
                Id = h.Id,
                PropertyId = h.PropertyId,
                Amount = h.Amount,
                Currency = h.Currency,
                EffectiveDate = h.EffectiveDate
            }).ToList()
    };
}
