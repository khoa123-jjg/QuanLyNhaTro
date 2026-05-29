using QLNhaTro.Models.ChuTro.YeuCauThue;

namespace QLNhaTro.Repositories.ChuTro;

public interface IChuTroYeuCauThueRepository
{
    Task<ChuTroYeuCauThueListPageViewModel> GetDanhSachAsync(int nguoiDungId, string? trangThai, int? nhaTroId, string? sapXep);

    Task<ChuTroChiTietYeuCauThueViewModel?> GetChiTietAsync(int id, int nguoiDungId);

    Task<ChuTroXuLyYeuCauThueViewModel?> GetXuLyAsync(int id, int nguoiDungId);

    Task<(bool Success, string Message)> XuLyYeuCauAsync(int id, int nguoiDungId, string trangThaiXuLy, string? ghiChuChuTro, string? lyDoTuChoi);
}
