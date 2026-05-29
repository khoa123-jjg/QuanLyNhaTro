using System.ComponentModel.DataAnnotations;
using QLNhaTro.Helpers.Constants;

namespace QLNhaTro.Models.ChuTro.YeuCauThue;

public class ChuTroXuLyYeuCauThueViewModel
{
    public int Id { get; set; }

    public string HoTenNguoiThue { get; set; } = string.Empty;

    public string SoDienThoaiLienHe { get; set; } = string.Empty;

    public string MaPhong { get; set; } = string.Empty;

    public string? TenPhong { get; set; }

    public string TenNhaTro { get; set; } = string.Empty;

    public decimal? GiaThueThang { get; set; }

    public string? AnhDaiDien { get; set; }

    public DateTime? NgayGui { get; set; }

    public DateTime? NgayMuonXemPhong { get; set; }

    public string TrangThaiHienTai { get; set; } = string.Empty;

    public string TrangThaiHienTaiText { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn trạng thái xử lý.")]
    public string TrangThaiXuLy { get; set; } = YeuCauThueStatus.ChuTroDongY;

    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChuChuTro { get; set; }

    [StringLength(500, ErrorMessage = "Lý do từ chối không được vượt quá 500 ký tự.")]
    public string? LyDoTuChoi { get; set; }
}
