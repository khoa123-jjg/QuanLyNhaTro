using System.ComponentModel.DataAnnotations;

namespace QLNhaTro.Models.Admin.HoSo;

public class AdminHoSoViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(150, ErrorMessage = "Email tối đa 150 ký tự.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    public string SoDienThoai { get; set; } = string.Empty;

    public string VaiTro { get; set; } = string.Empty;

    public DateTime? NgayTao { get; set; }

    public string? GhiChu { get; set; }

    public string? AnhDaiDien { get; set; }

    public AdminDoiMatKhauViewModel DoiMatKhau { get; set; } = new();
}
