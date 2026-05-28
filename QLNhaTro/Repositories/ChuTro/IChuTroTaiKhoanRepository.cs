using QLNhaTro.Models.ChuTro.CaiDat;

namespace QLNhaTro.Repositories.ChuTro;

public interface IChuTroTaiKhoanRepository
{
    Task<ChuTroCaiDatViewModel?> GetCaiDatAsync(string userId);

    Task<(bool Success, string Message)> CapNhatThongTinAsync(string userId, CapNhatThongTinViewModel model);

    Task<(bool Success, string Message)> DoiMatKhauAsync(string userId, DoiMatKhauViewModel model);
}
