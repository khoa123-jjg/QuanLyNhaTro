using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyNhaTro.Models.TienNghi;

public class TienNghiPhongPageViewModel
{
    public int? PhongTroIdDangChon { get; set; }

    public List<SelectListItem> DanhSachPhong { get; set; } = [];

    public List<TienNghiItemViewModel> DanhSachTienNghi { get; set; } = [];

    public List<TienNghiItemViewModel> TienNghiDaChon { get; set; } = [];
}

public class TienNghiItemViewModel
{
    public int Id { get; set; }

    public string TenTienNghi { get; set; } = string.Empty;

    public string Icon { get; set; } = "bi bi-check-circle";

    public bool DaChon { get; set; }
}
