using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNhaTro.Models.YeuCauThue;

public class YeuCauThueListPageViewModel
{
    public string? TuKhoa { get; set; }

    public string? TrangThai { get; set; }

    public List<SelectListItem> DanhSachTrangThai { get; set; } = new();

    public List<YeuCauThueListItemViewModel> YeuCaus { get; set; } = new();
}
