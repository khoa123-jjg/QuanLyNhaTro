namespace QLNhaTro.Models.NhaTro;

public class NhaTroListItemViewModel
{
    public int Id { get; set; }

    public string TenNhaTro { get; set; } = string.Empty;

    public string DiaChiDayDu { get; set; } = string.Empty;

    public string? TenDuong { get; set; }

    public string? TenXa { get; set; }

    public string? TenQuanHuyen { get; set; }

    public string TrangThai { get; set; } = string.Empty;

    public int SoPhong { get; set; }

    public DateTime NgayTao { get; set; }
}
