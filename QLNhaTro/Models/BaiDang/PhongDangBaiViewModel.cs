namespace QLNhaTro.Models.BaiDang
{
    public class PhongDangBaiViewModel
    {
        public int Id { get; set; }

        public string MaPhong { get; set; } = string.Empty;

        public string TenNhaTro { get; set; } = string.Empty;

        public decimal GiaThueThang { get; set; }

        public decimal DienTich { get; set; }

        public int? SoNguoiToiDa { get; set; }

        public string? AnhDaiDien { get; set; }
    }
}
