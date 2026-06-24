using PropertyInventory.Application.DTOs.Property;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Application.Interfaces;

public interface IPropertyRepository
{
    Task<(List<Property> Items, int TotalCount)> GetPagedAsync(PropertyFilter filter);
    Task<Property?> GetByIdAsync(Guid id, bool includeDetails = false);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(Property property);
    void Update(Property property);
    void Remove(Property property);

    Task AddPriceHistoryAsync(PropertyPriceHistory history);
    Task<PropertyOwnership?> GetCurrentOwnershipAsync(Guid propertyId);
    Task AddOwnershipAsync(PropertyOwnership ownership);

    /// <summary>All ownership records with Property and Contact eagerly loaded (for the dashboard).</summary>
    Task<List<PropertyOwnership>> GetAllOwnershipsWithDetailsAsync();

    Task<int> SaveChangesAsync();
}
