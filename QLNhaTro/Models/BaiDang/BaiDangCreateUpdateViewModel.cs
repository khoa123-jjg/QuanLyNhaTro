using QLNhaTro.Models.PhongTro;
using System.ComponentModel.DataAnnotations;

namespace QLNhaTro.Models.BaiDang
{
    public class BaiDangCreateUpdateViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà trọ.")]
        public int? NhaTroId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng đăng bài.")]
        public int? PhongTroId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài đăng.")]
        [StringLength(120, ErrorMessage = "Tiêu đề không được vượt quá 120 ký tự.")]
        public string TieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung bài đăng.")]
        public string NoiDung { get; set; } = string.Empty;
        public string TrangThaiDuyet { get; set; } = string.Empty;
        public List<PhongDangBaiViewModel> DanhSachPhong { get; set; } = new();

        public PhongDangBaiViewModel? PhongDangChon { get; set; }
    }
}
