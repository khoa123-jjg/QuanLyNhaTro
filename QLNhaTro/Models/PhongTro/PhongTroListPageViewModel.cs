using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNhaTro.Models.PhongTro;

public class PhongTroListPageViewModel
{
    public string? TuKhoa { get; set; }

    public int? NhaTroId { get; set; }

    public int? Tang { get; set; }

    public string? TrangThai { get; set; }

    public List<SelectListItem> DanhSachNhaTro { get; set; } = [];

    public List<SelectListItem> DanhSachTang { get; set; } = [];

    public List<SelectListItem> DanhSachTrangThai { get; set; } =
    [
        new SelectListItem("Trống", "TRONG"),
        new SelectListItem("Đã thuê", "DANG_THUE"),
        new SelectListItem("Đang sửa", "DANG_SUA"),
        new SelectListItem("Tạm ẩn", "TAM_AN")
    ];

    public List<PhongTroListItemViewModel> DanhSachPhong { get; set; } = [];
}
