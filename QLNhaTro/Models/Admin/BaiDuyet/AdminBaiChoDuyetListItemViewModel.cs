using QLNhaTro.Helpers.Constants;

namespace QLNhaTro.Models.Admin.BaiDuyet;

public class AdminBaiChoDuyetListItemViewModel
{
    public int Id { get; set; }

    public int Stt { get; set; }

    public string TieuDe { get; set; } = string.Empty;

    public string TenChuTro { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public DateTime? NgayGuiDuyet { get; set; }

    public string TrangThaiDuyet { get; set; } = string.Empty;

    public string TrangThaiText => BaiDangStatus.GetDisplayName(TrangThaiDuyet);

    public string? AnhDaiDien { get; set; }
}
