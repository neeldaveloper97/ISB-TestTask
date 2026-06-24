using PropertyInventory.Application.DTOs.Common;
using PropertyInventory.Application.DTOs.Property;

namespace PropertyInventory.Application.Services;

public interface IPropertyService
{
    Task<PagedResult<PropertyDto>> GetAllAsync(PropertyFilter filter);
    Task<PropertyDto?> GetByIdAsync(Guid id);
    Task<PropertyDto> CreateAsync(CreatePropertyDto dto);
    Task<List<PropertyDto>> CreateBulkAsync(IEnumerable<CreatePropertyDto> dtos);
    Task<PropertyDto?> UpdateAsync(Guid id, UpdatePropertyDto dto);
    Task<List<PropertyDto>> UpdateBulkAsync(IEnumerable<UpdatePropertyDto> dtos);
    Task<bool> DeleteAsync(Guid id);
    Task<PropertyDto?> AssignOwnerAsync(Guid propertyId, AssignOwnerDto dto);
}
