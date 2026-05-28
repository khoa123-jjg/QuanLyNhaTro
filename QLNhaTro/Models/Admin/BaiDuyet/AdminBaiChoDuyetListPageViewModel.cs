using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNhaTro.Models.Admin.BaiDuyet;

public class AdminBaiChoDuyetListPageViewModel
{
    public string? TuKhoa { get; set; }

    public int? NhaTroId { get; set; }

    public List<SelectListItem> DanhSachNhaTro { get; set; } = new();

    public List<AdminBaiChoDuyetListItemViewModel> BaiDangChoDuyet { get; set; } = new();
}
