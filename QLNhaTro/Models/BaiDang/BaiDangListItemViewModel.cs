namespace QLNhaTro.Models.BaiDang
{
    public class BaiDangListItemViewModel
    {
        public int Id { get; set; }

        public string TieuDe { get; set; } = string.Empty;

        public string TenNhaTro { get; set; } = string.Empty;

        public string MaPhong { get; set; } = string.Empty;

        public string? AnhDaiDien { get; set; }

        public decimal GiaThueThang { get; set; }

        public decimal DienTich { get; set; }

        public int? SoNguoiToiDa { get; set; }

        public string TrangThaiDuyet { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; }
    }
}
