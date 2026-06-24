using Microsoft.AspNetCore.Mvc;
using PropertyInventory.Application.DTOs.Common;
using PropertyInventory.Application.DTOs.Property;
using PropertyInventory.Application.Services;

namespace PropertyInventory.API.Controllers;

[ApiController]
[Route("api/properties")]
[Produces("application/json")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _service;

    public PropertiesController(IPropertyService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PropertyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PropertyDto>>> GetAll([FromQuery] PropertyFilter filter)
        => Ok(await _service.GetAllAsync(filter));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyDto>> GetById(Guid id)
    {
        var property = await _service.GetByIdAsync(id);
        return property is null ? NotFound() : Ok(property);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropertyDto>> Create([FromBody] CreatePropertyDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("bulk")]
    [ProducesResponseType(typeof(List<PropertyDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<PropertyDto>>> CreateBulk([FromBody] List<CreatePropertyDto> dtos)
    {
        var created = await _service.CreateBulkAsync(dtos);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyDto>> Update(Guid id, [FromBody] UpdatePropertyDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("bulk")]
    [ProducesResponseType(typeof(List<PropertyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PropertyDto>>> UpdateBulk([FromBody] List<UpdatePropertyDto> dtos)
        => Ok(await _service.UpdateBulkAsync(dtos));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/ownership")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyDto>> AssignOwner(Guid id, [FromBody] AssignOwnerDto dto)
    {
        var result = await _service.AssignOwnerAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }
}
