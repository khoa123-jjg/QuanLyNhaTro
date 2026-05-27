using System.ComponentModel.DataAnnotations;

namespace QLNhaTro.Models.Admin.TienNghi;

public class TienNghiFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên tiện nghi.")]
    [StringLength(150, ErrorMessage = "Tên tiện nghi không được vượt quá 150 ký tự.")]
    public string TenTienNghi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn trạng thái.")]
    public string TrangThai { get; set; } = string.Empty;
}
