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
    /// Trang gắn tiện nghi cho một phòng. Trả null nếu userId không hợp lệ hoặc phòng không thuộc chủ trọ.
    /// </summary>
    Task<TienNghiPhongPageViewModel?> GetTienNghiPhongPageAsync(string userId, int phongTroId);

    /// <summary>
    /// Lưu danh sách tiện nghi đã gắn cho phòng (thay thế toàn bộ liên kết cũ).
    /// </summary>
    Task<PhongTroManagementResult> LuuTienNghiPhongAsync(
        string userId,
        int phongTroId,
        List<int>? tienNghiIds);

    /// <summary>
    /// Trang quản lý ảnh của một phòng. Trả null nếu userId không hợp lệ hoặc phòng không thuộc chủ trọ.
    /// </summary>
    Task<HinhAnhPhongPageViewModel?> GetHinhAnhPhongPageAsync(string userId, int phongTroId);

    /// <summary>
    /// Kiểm tra phòng thuộc chủ trọ đang đăng nhập.
    /// </summary>
    Task<bool> PhongThuocChuTroAsync(int phongTroId, string userId);

    /// <summary>
    /// Đếm số ảnh hiện có của phòng.
    /// </summary>
    Task<int> DemSoAnhPhongAsync(int phongTroId);

    /// <summary>
    /// Thêm bản ghi ảnh (đường dẫn đã lưu trên wwwroot). Ảnh đầu tiên của phòng chưa có ảnh sẽ là ảnh bìa.
    /// </summary>
    Task<PhongTroManagementResult> ThemHinhAnhPhongAsync(
        string userId,
        int phongTroId,
        IReadOnlyList<string> duongDanAnh);
}
