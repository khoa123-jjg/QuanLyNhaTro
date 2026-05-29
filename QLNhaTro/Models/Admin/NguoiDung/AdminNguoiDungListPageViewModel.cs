using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNhaTro.Models.Admin.NguoiDung;

public class AdminNguoiDungListPageViewModel
{
    public string? TuKhoa { get; set; }

    public string? VaiTro { get; set; }

    public string? TrangThai { get; set; }

    public List<SelectListItem> DanhSachVaiTro { get; set; } = new();

    public List<SelectListItem> DanhSachTrangThai { get; set; } = new();

    public List<AdminNguoiDungListItemViewModel> NguoiDungs { get; set; } = new();
}
