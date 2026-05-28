namespace QLNhaTro.Models.Admin.NguoiDung;

public class AdminNguoiDungDetailViewModel
{
    public int Id { get; set; }

    public string HoTen { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? SoDienThoai { get; set; }

    public string VaiTro { get; set; } = string.Empty;

    public string VaiTroText { get; set; } = string.Empty;

    public string TrangThai { get; set; } = string.Empty;

    public string TrangThaiText { get; set; } = string.Empty;

    public DateTime? NgayTao { get; set; }

    public string? GhiChu { get; set; }

    // Tenant specific fields
    public string? NgheNghiep { get; set; }

    public string? NhuCauThue { get; set; }

    // Landlord specific fields
    public string? TrangThaiHoSo { get; set; }
}
