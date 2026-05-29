namespace QLNhaTro.Models.PhongTro;

public class PhongTroMapItemViewModel
{
    public int Id { get; set; }

    public string TieuDe { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public string DiaChi { get; set; } = string.Empty;

    public decimal GiaThue { get; set; }

    public decimal DienTich { get; set; }

    public decimal ViDo { get; set; }

    public decimal KinhDo { get; set; }

    public string? AnhDaiDien { get; set; }
}
