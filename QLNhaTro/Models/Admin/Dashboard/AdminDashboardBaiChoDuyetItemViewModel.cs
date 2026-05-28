namespace QLNhaTro.Models.Admin.Dashboard;

public class AdminDashboardBaiChoDuyetItemViewModel
{
    public int Id { get; set; }

    public int Stt { get; set; }

    public string TieuDe { get; set; } = string.Empty;

    public string TenNguoiDang { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public string MaPhong { get; set; } = string.Empty;

    public DateTime? NgayGuiDuyet { get; set; }

    public string? AnhDaiDien { get; set; }
}
