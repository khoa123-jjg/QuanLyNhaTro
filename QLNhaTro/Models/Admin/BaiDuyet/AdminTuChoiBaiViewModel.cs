using System.ComponentModel.DataAnnotations;
using QLNhaTro.Helpers.Constants;

namespace QLNhaTro.Models.Admin.BaiDuyet;

public class AdminTuChoiBaiViewModel
{
    public int Id { get; set; }

    public string TieuDe { get; set; } = string.Empty;

    public string TenChuTro { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public DateTime? NgayGuiDuyet { get; set; }

    public string TrangThaiText => BaiDangStatus.GetDisplayName(BaiDangStatus.ChoDuyet);

    [Required(ErrorMessage = "Vui lòng nhập lý do từ chối.")]
    [StringLength(500, ErrorMessage = "Lý do từ chối tối đa 500 ký tự.")]
    public string LyDoTuChoi { get; set; } = string.Empty;
}
