using QLNhaTro.Models.PhongTro;

namespace QLNhaTro.Repositories.PhongTro;

public interface IPhongTroRepository
{
    /// <summary>
    /// Lấy danh sách phòng nổi bật từ tin đăng đã duyệt (BAI_DANG).
    /// </summary>
    Task<List<PhongTroCardViewModel>> LayPhongNoiBatAsync(int soLuong = 4);

    /// <summary>
    /// Tìm phòng theo bộ lọc từ trang chủ / danh sách phòng.
    /// </summary>
    Task<PhongTroSearchViewModel> SearchPhongAsync(string? khuVuc, string? mucGia, string? dienTich);

    /// <summary>
    /// Lấy chi tiết phòng từ tin đăng đã duyệt.
    /// </summary>
    Task<PhongTroDetailViewModel?> GetChiTietPhongAsync(int id);

    Task<PhongTroMapPageViewModel> GetBanDoPhongAsync(int? phongTroId = null);
}
