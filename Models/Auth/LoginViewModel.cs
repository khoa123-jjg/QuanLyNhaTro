using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaTro.Models.Auth;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    public string MatKhau { get; set; } = string.Empty;

    public bool GhiNhoDangNhap { get; set; }
}
