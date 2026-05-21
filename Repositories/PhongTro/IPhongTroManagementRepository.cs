using Microsoft.AspNetCore.Mvc.Rendering;
using QLNhaTro.Models.PhongTro;

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

    /// <summary>
    /// Form thêm (id null) hoặc sửa phòng thuộc chủ trọ. Trả null nếu phòng không thuộc chủ trọ.
    /// </summary>
    Task<PhongTroCreateUpdateViewModel?> GetPhongFormAsync(int? id, string userId);

    Task<PhongTroManagementResult> CreatePhong(string userId, PhongTroCreateUpdateViewModel model);

    Task<PhongTroManagementResult> UpdatePhongAsync(string userId, PhongTroCreateUpdateViewModel model);
}
