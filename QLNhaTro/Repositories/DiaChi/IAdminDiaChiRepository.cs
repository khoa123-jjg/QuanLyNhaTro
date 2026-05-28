using QLNhaTro.Models.Admin.DiaChi;

namespace QLNhaTro.Repositories.DiaChi;

public interface IAdminDiaChiRepository
{
    Task<XaPageViewModel> GetXaPageAsync(string? tuKhoa, int? quanHuyenId, int? id);
    Task<(bool Success, string Message)> LuuXaAsync(XaFormViewModel form);
    Task<(bool Success, string Message)> XoaXaAsync(int id);

    Task<DuongPhoPageViewModel> GetDuongPhoPageAsync(string? tuKhoa, int? xaId, int? id);
    Task<(bool Success, string Message)> LuuDuongPhoAsync(DuongPhoFormViewModel form);
    Task<(bool Success, string Message)> XoaDuongPhoAsync(int id);

    Task<QuanHuyenPageViewModel> GetQuanHuyenPageAsync(string? tuKhoa, int? id);
    Task<(bool Success, string Message)> LuuQuanHuyenAsync(QuanHuyenFormViewModel form);
    Task<(bool Success, string Message)> XoaQuanHuyenAsync(int id);
}
