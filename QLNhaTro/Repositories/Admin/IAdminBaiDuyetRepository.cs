using QLNhaTro.Models.Admin.BaiDuyet;

namespace QLNhaTro.Repositories.Admin;

public interface IAdminBaiDuyetRepository
{
    Task<AdminBaiChoDuyetListPageViewModel> GetDanhSachChoDuyetAsync(string? tuKhoa, int? nhaTroId);

    Task<AdminChiTietBaiChoDuyetViewModel?> GetChiTietChoDuyetAsync(int id);

    Task<AdminTuChoiBaiViewModel?> GetTuChoiViewModelAsync(int id);

    Task<(bool Success, string Message)> DuyetBaiAsync(int id, string adminUserId);

    Task<(bool Success, string Message)> TuChoiBaiAsync(int id, string adminUserId, string lyDoTuChoi);
}
