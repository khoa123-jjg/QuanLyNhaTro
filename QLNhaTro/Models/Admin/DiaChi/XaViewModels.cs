using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNhaTro.Models.Admin.DiaChi;

public class XaListItemViewModel
{
    public int Id { get; set; }

    public string TenXa { get; set; } = string.Empty;

    public int QuanHuyenId { get; set; }

    public string TenQuanHuyen { get; set; } = string.Empty;
}

public class XaFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên phường/xã.")]
    [StringLength(150, ErrorMessage = "Tên phường/xã tối đa 150 ký tự.")]
    public string TenXa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn quận/huyện.")]
    public int? QuanHuyenId { get; set; }
}

public class XaPageViewModel
{
    public List<XaListItemViewModel> DanhSachXa { get; set; } = [];

    public XaFormViewModel Form { get; set; } = new();

    public List<SelectListItem> DanhSachQuanHuyen { get; set; } = [];

    public string? TuKhoa { get; set; }

    public int? QuanHuyenId { get; set; }
}

public class DuongPhoListItemViewModel
{
    public int Id { get; set; }

    public string TenDuong { get; set; } = string.Empty;

    public int XaId { get; set; }

    public string TenXa { get; set; } = string.Empty;

    public string TenQuanHuyen { get; set; } = string.Empty;
}

public class DuongPhoFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên đường.")]
    [StringLength(150, ErrorMessage = "Tên đường tối đa 150 ký tự.")]
    public string TenDuong { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn phường/xã.")]
    public int? XaId { get; set; }
}

public class DuongPhoPageViewModel
{
    public List<DuongPhoListItemViewModel> DanhSachDuongPho { get; set; } = [];

    public DuongPhoFormViewModel Form { get; set; } = new();

    public List<SelectListItem> DanhSachXa { get; set; } = [];

    public string? TuKhoa { get; set; }

    public int? XaId { get; set; }
}
