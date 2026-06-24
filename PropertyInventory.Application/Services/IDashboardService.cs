using PropertyInventory.Application.DTOs.Dashboard;

namespace PropertyInventory.Application.Services;

public interface IDashboardService
{
    Task<List<DashboardRowDto>> GetDashboardAsync();
}
