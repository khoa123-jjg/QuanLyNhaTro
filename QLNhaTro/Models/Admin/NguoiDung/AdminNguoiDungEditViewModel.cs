using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNhaTro.Models.Admin.NguoiDung;

public class AdminNguoiDungEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(150, ErrorMessage = "Email tối đa 150 ký tự.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    public string? SoDienThoai { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trạng thái.")]
    public string TrangThai { get; set; } = string.Empty;

    public string? GhiChu { get; set; }

    public string VaiTro { get; set; } = string.Empty; // ADMIN, CHU_TRO, NGUOI_THUE
    public string VaiTroText { get; set; } = string.Empty;

    // Tenant specific fields
    public string? NgheNghiep { get; set; }
    public string? NhuCauThue { get; set; }

    // Landlord specific fields
    public string? TrangThaiHoSo { get; set; }

    public List<SelectListItem> DanhSachTrangThai { get; set; } = [];
    public List<SelectListItem> DanhSachTrangThaiHoSo { get; set; } = [];
}
