namespace QLNhaTroV7.ViewModels.Public;

public class PhongCardViewModel
{
    public int BaiDangId { get; set; }

    public int PhongTroId { get; set; }

    public string TieuDe { get; set; } = string.Empty;

    public string TenPhong { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public string DiaChiNgan { get; set; } = string.Empty;

    public decimal DienTich { get; set; }

    public decimal GiaThueThang { get; set; }

    public string TrangThaiPhong { get; set; } = string.Empty;

    public string? AnhDaiDien { get; set; }

    public List<string> TienNghi { get; set; } = new();
}