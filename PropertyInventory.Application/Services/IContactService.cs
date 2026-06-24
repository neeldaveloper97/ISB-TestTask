using PropertyInventory.Application.DTOs.Common;
using PropertyInventory.Application.DTOs.Contact;

namespace PropertyInventory.Application.Services;

public interface IContactService
{
    Task<PagedResult<ContactDto>> GetAllAsync(ContactFilter filter);
    Task<ContactDto?> GetByIdAsync(Guid id);
    Task<ContactDto> CreateAsync(CreateContactDto dto);
    Task<List<ContactDto>> CreateBulkAsync(IEnumerable<CreateContactDto> dtos);
    Task<ContactDto?> UpdateAsync(Guid id, UpdateContactDto dto);
    Task<List<ContactDto>> UpdateBulkAsync(IEnumerable<UpdateContactDto> dtos);
}
