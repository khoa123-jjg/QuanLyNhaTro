namespace QLNhaTro.Models.YeuCauThue;

public class YeuCauThueListItemViewModel
{
    public int Id { get; set; }

    public string TieuDePhong { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public string DiaChi { get; set; } = string.Empty;

    public decimal GiaThue { get; set; }

    public DateTime? NgayGui { get; set; }

    public DateTime? NgayMuonXemPhong { get; set; }

    public string TrangThai { get; set; } = string.Empty;

    public string TrangThaiText { get; set; } = string.Empty;

    public string? AnhDaiDien { get; set; }
}
