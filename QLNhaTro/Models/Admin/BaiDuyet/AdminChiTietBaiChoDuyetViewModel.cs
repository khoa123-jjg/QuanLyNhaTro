using QLNhaTro.Helpers.Constants;

namespace QLNhaTro.Models.Admin.BaiDuyet;

public class AdminChiTietBaiChoDuyetViewModel
{
    public int Id { get; set; }

    public string TieuDe { get; set; } = string.Empty;

    public string NoiDung { get; set; } = string.Empty;

    public string TenChuTro { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public string MaPhong { get; set; } = string.Empty;

    public string? TenPhong { get; set; }

    public string DiaChi { get; set; } = string.Empty;

    public decimal DienTich { get; set; }

    public decimal GiaThueThang { get; set; }

    public decimal TienCoc { get; set; }

    public int? SoNguoiToiDa { get; set; }

    public DateTime? NgayGuiDuyet { get; set; }

    public string TrangThaiDuyet { get; set; } = string.Empty;

    public string TrangThaiText => BaiDangStatus.GetDisplayName(TrangThaiDuyet);

    public List<string> DanhSachAnh { get; set; } = new();

    public List<string> DanhSachTienNghi { get; set; } = new();
}
