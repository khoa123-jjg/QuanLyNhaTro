using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNhaTro.Models.NhaTro;

public class NhaTroCreateUpdateViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên nhà trọ.")]
    [MaxLength(200, ErrorMessage = "Tên nhà trọ tối đa 200 ký tự.")]
    [Display(Name = "Tên nhà trọ")]
    public string TenNhaTro { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [MaxLength(50, ErrorMessage = "Số nhà tối đa 50 ký tự.")]
    [Display(Name = "Số nhà")]
    public string? SoNha { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ chi tiết.")]
    [MaxLength(500, ErrorMessage = "Địa chỉ chi tiết tối đa 500 ký tự.")]
    [Display(Name = "Địa chỉ chi tiết")]
    public string DiaChiChiTiet { get; set; } = string.Empty;

    [Display(Name = "Quận / huyện")]
    public int? QuanHuyenId { get; set; }

    [Display(Name = "Phường / xã")]
    public int? XaId { get; set; }

    [Display(Name = "Đường / phố")]
    public int? DuongPhoId { get; set; }

    [Display(Name = "Vĩ độ")]
    public decimal? ViDo { get; set; }

    [Display(Name = "Kinh độ")]
    public decimal? KinhDo { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trạng thái.")]
    [RegularExpression("^(HOAT_DONG|TAM_AN|NGUNG_HOAT_DONG)$", ErrorMessage = "Trạng thái không hợp lệ.")]
    [Display(Name = "Trạng thái")]
    public string TrangThai { get; set; } = "HOAT_DONG";

    public List<SelectListItem> DanhSachQuanHuyen { get; set; } = [];

    public List<SelectListItem> DanhSachXa { get; set; } = [];

    public List<SelectListItem> DanhSachDuongPho { get; set; } = [];

    public List<SelectListItem> DanhSachTrangThai { get; set; } =
    [
        new SelectListItem("Đang hoạt động", "HOAT_DONG"),
        new SelectListItem("Tạm ẩn", "TAM_AN"),
        new SelectListItem("Ngừng hoạt động", "NGUNG_HOAT_DONG")
    ];
}
