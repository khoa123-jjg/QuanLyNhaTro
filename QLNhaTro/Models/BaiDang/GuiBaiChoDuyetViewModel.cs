namespace QLNhaTro.Models.BaiDang;

public class GuiBaiChoDuyetViewModel
{
    public int Id { get; set; }

    public string TieuDe { get; set; } = string.Empty;

    public string NoiDung { get; set; } = string.Empty;

    public string TrangThaiDuyet { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public string MaPhong { get; set; } = string.Empty;

    public string? AnhDaiDien { get; set; }

    public decimal GiaThueThang { get; set; }

    public decimal DienTich { get; set; }

    public int? SoNguoiToiDa { get; set; }

    public string? DiaChi { get; set; }

    public string? SoDienThoaiLienHe { get; set; }

    public List<string> TienNghi { get; set; } = [];
}
