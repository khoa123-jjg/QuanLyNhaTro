using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNhaTro.Models.ChuTro.YeuCauThue;

public class ChuTroYeuCauThueListPageViewModel
{
    public string? TrangThai { get; set; }

    public int? NhaTroId { get; set; }

    public string? SapXep { get; set; }

    public List<SelectListItem> DanhSachTrangThai { get; set; } = new();

    public List<SelectListItem> DanhSachNhaTro { get; set; } = new();

    public List<ChuTroYeuCauThueListItemViewModel> YeuCaus { get; set; } = new();
}
