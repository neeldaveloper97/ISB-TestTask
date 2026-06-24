using PropertyInventory.Application.DTOs.Contact;
using PropertyInventory.Domain.Entities;

namespace PropertyInventory.Application.Interfaces;

public interface IContactRepository
{
    Task<(List<Contact> Items, int TotalCount)> GetPagedAsync(ContactFilter filter);
    Task<Contact?> GetByIdAsync(Guid id, bool includeOwnerships = false);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(Contact contact);
    void Update(Contact contact);
    Task<int> SaveChangesAsync();
}
