using QLNhaTro.Models.Admin.NguoiDung;

namespace QLNhaTro.Repositories.Admin;

public interface IAdminNguoiDungRepository
{
    Task<AdminNguoiDungListPageViewModel> GetDanhSachNguoiDungAsync(
        string? tuKhoa,
        string? vaiTro,
        string? trangThai);

    Task<AdminNguoiDungDetailViewModel?> GetChiTietNguoiDungAsync(int id);

    Task<(bool Success, string Message)> KhoaMoKhoaNguoiDungAsync(int id, int adminId);

    Task<(bool Success, string Message)> XoaNguoiDungAsync(int id, int adminId);
}
