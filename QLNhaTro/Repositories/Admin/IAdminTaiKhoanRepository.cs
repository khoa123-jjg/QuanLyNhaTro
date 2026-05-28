using QLNhaTro.Models.Admin.HoSo;

namespace QLNhaTro.Repositories.Admin;

public interface IAdminTaiKhoanRepository
{
    Task<AdminHoSoViewModel?> GetHoSoAsync(string userId);

    Task<(bool Success, string Message)> DoiMatKhauAsync(string userId, AdminDoiMatKhauViewModel model);

    Task<(bool Success, string Message)> CapNhatHoSoAsync(string userId, AdminHoSoViewModel model);
}
