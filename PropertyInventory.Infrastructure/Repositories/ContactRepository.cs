using Microsoft.EntityFrameworkCore;
using PropertyInventory.Application.DTOs.Contact;
using PropertyInventory.Application.Interfaces;
using PropertyInventory.Domain.Entities;
using PropertyInventory.Infrastructure.Data;

namespace PropertyInventory.Infrastructure.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _db;

    public ContactRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Contact> Items, int TotalCount)> GetPagedAsync(ContactFilter filter)
    {
        var query = _db.Contacts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.FirstName))
            query = query.Where(c => c.FirstName.Contains(filter.FirstName));
        if (!string.IsNullOrWhiteSpace(filter.LastName))
            query = query.Where(c => c.LastName.Contains(filter.LastName));
        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(c => c.EmailAddress.Contains(filter.Email));

        var total = await query.CountAsync();

        var page = filter.Page < 1 ? 1 : filter.Page;
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = await query
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Contact?> GetByIdAsync(Guid id, bool includeOwnerships = false)
    {
        if (!includeOwnerships)
            return await _db.Contacts.FirstOrDefaultAsync(c => c.Id == id);

        return await _db.Contacts
            .Include(c => c.Ownerships).ThenInclude(o => o.Property)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<bool> ExistsAsync(Guid id) => _db.Contacts.AnyAsync(c => c.Id == id);

    public async Task AddAsync(Contact contact) => await _db.Contacts.AddAsync(contact);

    public void Update(Contact contact) => _db.Contacts.Update(contact);

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}
