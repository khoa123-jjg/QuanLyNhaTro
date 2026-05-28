using System.ComponentModel.DataAnnotations;

namespace QLNhaTro.Models.Admin.DiaChi;

public class QuanHuyenListItemViewModel
{
    public int Id { get; set; }
    public string TenQuanHuyen { get; set; } = string.Empty;
    public string ThanhPho { get; set; } = string.Empty;
}

public class QuanHuyenFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên quận/huyện.")]
    [StringLength(100, ErrorMessage = "Tên quận/huyện tối đa 100 ký tự.")]
    public string TenQuanHuyen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên thành phố.")]
    [StringLength(100, ErrorMessage = "Tên thành phố tối đa 100 ký tự.")]
    public string ThanhPho { get; set; } = "Đà Nẵng";
}

public class QuanHuyenPageViewModel
{
    public List<QuanHuyenListItemViewModel> DanhSachQuanHuyen { get; set; } = [];
    public QuanHuyenFormViewModel Form { get; set; } = new();
    public string? TuKhoa { get; set; }
}
