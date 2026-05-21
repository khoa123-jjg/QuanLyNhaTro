using Microsoft.AspNetCore.Mvc.Rendering;
using QLNhaTro.Models.PhongTro;
using QuanLyNhaTro.Models.TienNghi;

namespace QLNhaTro.Repositories.PhongTro;

public class PhongTroManagementResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;
}

public interface IPhongTroManagementRepository
{
    /// <summary>
    /// Dropdown nhà trọ thuộc chủ trọ đang đăng nhập.
    /// </summary>
    Task<List<SelectListItem>> GetDanhSachNhaTroCuaChuTroAsync(string userId);

    Task<PhongTroListPageViewModel> GetDanhSachPhongAsync(
        string userId,
        string? tuKhoa,
        int? nhaTroId,
        int? tang,
        string? trangThai);

    /// <summary>
    /// Form thêm (id null) hoặc sửa phòng thuộc chủ trọ. Trả null nếu phòng không thuộc chủ trọ.
    /// </summary>
    Task<PhongTroCreateUpdateViewModel?> GetPhongFormAsync(int? id, string userId);

    Task<PhongTroManagementResult> CreatePhong(string userId, PhongTroCreateUpdateViewModel model);

    Task<PhongTroManagementResult> UpdatePhongAsync(string userId, PhongTroCreateUpdateViewModel model);

    /// <summary>
    /// Trang gán tiện nghi: danh sách phòng, catalog tiện nghi, phòng đang chọn (chưa lưu).
    /// </summary>
    Task<TienNghiPhongPageViewModel?> GetGanTienNghiTrangAsync(int? phongTroId, string userId);
}
