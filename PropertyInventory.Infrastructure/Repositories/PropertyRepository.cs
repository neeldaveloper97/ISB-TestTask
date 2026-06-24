using Microsoft.EntityFrameworkCore;
using PropertyInventory.Application.DTOs.Property;
using PropertyInventory.Application.Interfaces;
using PropertyInventory.Domain.Entities;
using PropertyInventory.Infrastructure.Data;

namespace PropertyInventory.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly AppDbContext _db;

    public PropertyRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Property> Items, int TotalCount)> GetPagedAsync(PropertyFilter filter)
    {
        var query = _db.Properties.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(p => p.Name.Contains(filter.Name));
        if (!string.IsNullOrWhiteSpace(filter.Address))
            query = query.Where(p => p.Address.Contains(filter.Address));
        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        var total = await query.CountAsync();

        var page = filter.Page < 1 ? 1 : filter.Page;
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.Ownerships).ThenInclude(o => o.Contact)
            .Include(p => p.PriceHistories)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Property?> GetByIdAsync(Guid id, bool includeDetails = false)
    {
        if (!includeDetails)
            return await _db.Properties.FirstOrDefaultAsync(p => p.Id == id);

        return await _db.Properties
            .Include(p => p.Ownerships).ThenInclude(o => o.Contact)
            .Include(p => p.PriceHistories)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<bool> ExistsAsync(Guid id) => _db.Properties.AnyAsync(p => p.Id == id);

    public async Task AddAsync(Property property) => await _db.Properties.AddAsync(property);

    public void Update(Property property) => _db.Properties.Update(property);

    public void Remove(Property property) => _db.Properties.Remove(property);

    public async Task AddPriceHistoryAsync(PropertyPriceHistory history) =>
        await _db.PropertyPriceHistories.AddAsync(history);

    public Task<PropertyOwnership?> GetCurrentOwnershipAsync(Guid propertyId) =>
        _db.PropertyOwnerships
            .Where(o => o.PropertyId == propertyId && o.EffectiveTill == null)
            .OrderByDescending(o => o.EffectiveFrom)
            .FirstOrDefaultAsync();

    public async Task AddOwnershipAsync(PropertyOwnership ownership) =>
        await _db.PropertyOwnerships.AddAsync(ownership);

    public async Task<List<PropertyOwnership>> GetAllOwnershipsWithDetailsAsync() =>
        await _db.PropertyOwnerships
            .AsNoTracking()
            .Include(o => o.Property)
            .Include(o => o.Contact)
            .ToListAsync();

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}
