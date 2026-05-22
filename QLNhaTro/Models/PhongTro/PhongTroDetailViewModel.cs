namespace QLNhaTro.Models.PhongTro;

public class PhongTroDetailViewModel
{
    public int Id { get; set; }

    public string TieuDe { get; set; } = string.Empty;

    public decimal GiaThue { get; set; }

    public decimal DienTich { get; set; }

    public string DiaChi { get; set; } = string.Empty;

    public string MoTa { get; set; } = string.Empty;

    public List<string> DanhSachAnh { get; set; } = [];

    public List<string> TienNghi { get; set; } = [];

    public string TenNhaTro { get; set; } = string.Empty;

    public string SoDienThoaiLienHe { get; set; } = string.Empty;
}
