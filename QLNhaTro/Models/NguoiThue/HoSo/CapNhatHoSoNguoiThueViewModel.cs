using System.ComponentModel.DataAnnotations;

namespace QLNhaTro.Models.NguoiThue.HoSo;

public class CapNhatHoSoNguoiThueViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(150, ErrorMessage = "Họ tên tối đa 150 ký tự.")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(150, ErrorMessage = "Email tối đa 150 ký tự.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    public string? SoDienThoai { get; set; }

    [StringLength(150, ErrorMessage = "Nghề nghiệp tối đa 150 ký tự.")]
    public string? NgheNghiep { get; set; }

    [StringLength(255, ErrorMessage = "Nhu cầu thuê tối đa 255 ký tự.")]
    public string? NhuCauThue { get; set; }
}
