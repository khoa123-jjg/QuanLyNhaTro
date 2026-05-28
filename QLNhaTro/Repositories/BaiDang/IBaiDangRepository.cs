using QLNhaTro.Models.BaiDang;

namespace QLNhaTro.Repositories.BaiDang;

public interface IBaiDangRepository
{
    Task<BaiDangListPageViewModel> GetDanhSachBaiDangAsync(
        string userId,
        string? tuKhoa,
        int? nhaTroId,
        string? trangThaiDuyet);

    Task<BaiDangCreateUpdateViewModel> GetCreateModelAsync(string userId, int? nhaTroId = null);

    Task<BaiDangCreateUpdateViewModel?> GetUpdateModelAsync(string userId, int baiDangId);

    Task<PhongDangBaiViewModel?> GetThongTinPhongAsync(string userId, int phongTroId);

    Task<bool> LuuNhapAsync(string userId, BaiDangCreateUpdateViewModel model);

    Task<int?> LuuNhapVaTraVeIdAsync(string userId, BaiDangCreateUpdateViewModel model);

    Task<GuiBaiChoDuyetViewModel?> GetGuiBaiChoDuyetAsync(string userId, int baiDangId);

    Task<bool> XacNhanGuiChoDuyetAsync(string userId, int baiDangId);
}
