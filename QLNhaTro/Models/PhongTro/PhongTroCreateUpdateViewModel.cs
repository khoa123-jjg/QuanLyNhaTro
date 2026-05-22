using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNhaTro.Models.PhongTro;

public class PhongTroCreateUpdateViewModel
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhà trọ.")]
    [Display(Name = "Nhà trọ")]
    public int NhaTroId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã phòng.")]
    [MaxLength(50, ErrorMessage = "Mã phòng tối đa 50 ký tự.")]
    [Display(Name = "Mã phòng")]
    public string MaPhong { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên phòng.")]
    [MaxLength(200, ErrorMessage = "Tên phòng tối đa 200 ký tự.")]
    [Display(Name = "Tên phòng")]
    public string TenPhong { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Tầng phải lớn hơn hoặc bằng 0.")]
    [Display(Name = "Tầng")]
    public int? Tang { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập diện tích.")]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Diện tích phải lớn hơn 0.")]
    [Display(Name = "Diện tích (m²)")]
    public decimal DienTich { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập giá thuê tháng.")]
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Giá thuê phải lớn hơn hoặc bằng 0.")]
    [Display(Name = "Giá thuê / tháng")]
    public decimal GiaThueThang { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tiền cọc.")]
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Tiền cọc phải lớn hơn hoặc bằng 0.")]
    [Display(Name = "Tiền cọc")]
    public decimal TienCoc { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số người tối đa phải lớn hơn 0.")]
    [Display(Name = "Số người tối đa")]
    public int? SoNguoiToiDa { get; set; }

    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trạng thái.")]
    [RegularExpression("^(TRONG|DANG_THUE|DANG_SUA|TAM_AN)$", ErrorMessage = "Trạng thái không hợp lệ.")]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = "TRONG";

    [MaxLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public List<SelectListItem> DanhSachNhaTro { get; set; } = [];

    public List<SelectListItem> DanhSachTrangThai { get; set; } =
    [
        new SelectListItem("Trống", "TRONG"),
        new SelectListItem("Đang thuê", "DANG_THUE"),
        new SelectListItem("Đang sửa", "DANG_SUA"),
        new SelectListItem("Tạm ẩn", "TAM_AN")
    ];
}
