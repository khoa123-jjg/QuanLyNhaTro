using QLNhaTro.Models.NguoiThue.HoSo;

namespace QLNhaTro.Repositories.NguoiThue;

public interface INguoiThueHoSoRepository
{
    Task<NguoiThueHoSoViewModel?> GetHoSoAsync(string userId);

    Task<(bool Success, string Message)> CapNhatHoSoAsync(string userId, CapNhatHoSoNguoiThueViewModel model);

    Task<(bool Success, string Message)> DoiMatKhauAsync(string userId, DoiMatKhauNguoiThueViewModel model);
}
