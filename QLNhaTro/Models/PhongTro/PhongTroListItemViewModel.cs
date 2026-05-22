namespace QLNhaTro.Models.PhongTro;

public class PhongTroListItemViewModel
{
    public int Id { get; set; }

    public string MaPhong { get; set; } = string.Empty;

    public string TenPhong { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public int? Tang { get; set; }

    public decimal DienTich { get; set; }

    public decimal GiaThueThang { get; set; }

    public decimal TienCoc { get; set; }

    public int? SoNguoiToiDa { get; set; }

    public string TrangThai { get; set; } = string.Empty;

    public string? GhiChu { get; set; }

    public string? MoTa { get; set; }
}
