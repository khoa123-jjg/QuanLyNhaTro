namespace QLNhaTro.Models.Admin.HoSo;

public class AdminHoSoViewModel
{
    public string HoTen { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SoDienThoai { get; set; } = string.Empty;

    public string VaiTro { get; set; } = string.Empty;

    public DateTime? NgayTao { get; set; }

    public string? GhiChu { get; set; }

    public string? AnhDaiDien { get; set; }

    public AdminDoiMatKhauViewModel DoiMatKhau { get; set; } = new();
}
