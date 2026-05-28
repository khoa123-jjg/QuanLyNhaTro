namespace QLNhaTro.Models.Admin.NguoiDung;

public class AdminNguoiDungListItemViewModel
{
    public int Id { get; set; }

    public int Stt { get; set; }

    public string HoTen { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? SoDienThoai { get; set; }

    public string VaiTro { get; set; } = string.Empty;

    public string VaiTroText { get; set; } = string.Empty;

    public string TrangThai { get; set; } = string.Empty;

    public string TrangThaiText { get; set; } = string.Empty;

    public DateTime? NgayTao { get; set; }
}
