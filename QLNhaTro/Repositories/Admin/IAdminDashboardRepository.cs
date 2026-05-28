using QLNhaTro.Models.Admin.Dashboard;

namespace QLNhaTro.Repositories.Admin;

public interface IAdminDashboardRepository
{
    Task<AdminDashboardViewModel> GetDashboardAsync();
}
