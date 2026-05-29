namespace QLNhaTro.Models.ChuTro.YeuCauThue;

public class ChuTroYeuCauThueListItemViewModel
{
    public int Id { get; set; }

    public string HoTenNguoiThue { get; set; } = string.Empty;

    public string SoDienThoaiLienHe { get; set; } = string.Empty;

    public string? EmailNguoiThue { get; set; }

    public int PhongTroId { get; set; }

    public string MaPhong { get; set; } = string.Empty;

    public string TenPhong { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public decimal GiaThueThang { get; set; }

    public string? AnhDaiDien { get; set; }

    public DateTime? NgayGui { get; set; }

    public DateTime? NgayMuonXemPhong { get; set; }

    public string TrangThai { get; set; } = string.Empty;

    public string TrangThaiText { get; set; } = string.Empty;

    public bool CoTheXuLy { get; set; }
}
