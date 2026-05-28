using System.ComponentModel.DataAnnotations;

namespace QLNhaTro.Models.YeuCauThue;

public class GuiYeuCauThueViewModel
{
    public int PhongTroId { get; set; }

    public string TieuDePhong { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public string DiaChi { get; set; } = string.Empty;

    public decimal GiaThue { get; set; }

    public decimal DienTich { get; set; }

    public string? AnhDaiDien { get; set; }

    [Display(Name = "Họ tên liên hệ")]
    [Required(ErrorMessage = "Vui lòng nhập họ tên liên hệ.")]
    [StringLength(150, ErrorMessage = "Họ tên không được vượt quá 150 ký tự.")]
    public string HoTenLienHe { get; set; } = string.Empty;

    [Display(Name = "Số điện thoại liên hệ")]
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại liên hệ.")]
    [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
    public string SoDienThoaiLienHe { get; set; } = string.Empty;

    [Display(Name = "Ngày muốn xem phòng")]
    [Required(ErrorMessage = "Vui lòng chọn ngày muốn xem phòng.")]
    public DateTime? NgayMuonXemPhong { get; set; }

    [Display(Name = "Lời nhắn")]
    [StringLength(500, ErrorMessage = "Lời nhắn không được vượt quá 500 ký tự.")]
    public string? LoiNhan { get; set; }
}
