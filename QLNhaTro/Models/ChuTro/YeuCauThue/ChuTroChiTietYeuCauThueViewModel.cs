namespace QLNhaTro.Models.ChuTro.YeuCauThue;

public class ChuTroChiTietYeuCauThueViewModel
{
    public int Id { get; set; }

    public string HoTenNguoiThue { get; set; } = string.Empty;

    public string SoDienThoaiLienHe { get; set; } = string.Empty;

    public string? EmailNguoiThue { get; set; }

    public int PhongTroId { get; set; }

    public string MaPhong { get; set; } = string.Empty;

    public string TenPhong { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public string DiaChi { get; set; } = string.Empty;

    public decimal GiaThueThang { get; set; }

    public decimal DienTich { get; set; }

    public int? SoNguoiToiDa { get; set; }

    public string? AnhDaiDien { get; set; }

    public string? LoiNhan { get; set; }

    public DateTime? NgayGui { get; set; }

    public DateTime? NgayMuonXemPhong { get; set; }

    public string TrangThai { get; set; } = string.Empty;

    public string TrangThaiText { get; set; } = string.Empty;

    public bool CoTheXuLy { get; set; }

    public string? GhiChuChuTro { get; set; }

    public string? LyDoTuChoi { get; set; }
}
