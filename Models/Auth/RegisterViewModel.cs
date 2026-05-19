using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaTro.Models.Auth;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    public string SoDienThoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [Compare(nameof(MatKhau), ErrorMessage = "Xác nhận mật khẩu không khớp.")]
    public string XacNhanMatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn vai trò đăng ký.")]
    [RegularExpression("^(NGUOI_THUE|CHU_TRO)$", ErrorMessage = "Vai trò đăng ký chỉ được là NGUOI_THUE hoặc CHU_TRO.")]
    public string VaiTroDangKy { get; set; } = "NGUOI_THUE";
}
