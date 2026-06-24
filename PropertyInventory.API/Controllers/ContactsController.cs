using Microsoft.AspNetCore.Mvc;
using PropertyInventory.Application.DTOs.Common;
using PropertyInventory.Application.DTOs.Contact;
using PropertyInventory.Application.Services;

namespace PropertyInventory.API.Controllers;

[ApiController]
[Route("api/contacts")]
[Produces("application/json")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _service;

    public ContactsController(IContactService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ContactDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ContactDto>>> GetAll([FromQuery] ContactFilter filter)
        => Ok(await _service.GetAllAsync(filter));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDto>> GetById(Guid id)
    {
        var contact = await _service.GetByIdAsync(id);
        return contact is null ? NotFound() : Ok(contact);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContactDto>> Create([FromBody] CreateContactDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("bulk")]
    [ProducesResponseType(typeof(List<ContactDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ContactDto>>> CreateBulk([FromBody] List<CreateContactDto> dtos)
    {
        var created = await _service.CreateBulkAsync(dtos);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDto>> Update(Guid id, [FromBody] UpdateContactDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("bulk")]
    [ProducesResponseType(typeof(List<ContactDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ContactDto>>> UpdateBulk([FromBody] List<UpdateContactDto> dtos)
        => Ok(await _service.UpdateBulkAsync(dtos));
}
