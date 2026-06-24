using Microsoft.AspNetCore.Mvc;
using PropertyInventory.Application.DTOs.Dashboard;
using PropertyInventory.Application.Services;

namespace PropertyInventory.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<DashboardRowDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DashboardRowDto>>> Get()
        => Ok(await _service.GetDashboardAsync());
}
