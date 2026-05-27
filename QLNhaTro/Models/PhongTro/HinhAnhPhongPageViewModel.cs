namespace QLNhaTro.Models.PhongTro;

public class HinhAnhPhongPageViewModel
{
    public const int MaxAnhMoiPhong = 20;

    public int PhongTroId { get; set; }

    public HinhAnhPhongThongTinViewModel PhongDangChon { get; set; } = null!;

    public List<HinhAnhItemViewModel> DanhSachAnh { get; set; } = [];
}

public class HinhAnhPhongThongTinViewModel
{
    public int Id { get; set; }

    public string MaPhong { get; set; } = string.Empty;

    public string TenPhong { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public int? Tang { get; set; }

    public decimal DienTich { get; set; }

    public string TrangThai { get; set; } = string.Empty;

    public string TrangThaiHienThi { get; set; } = string.Empty;
}

public class HinhAnhItemViewModel
{
    public int Id { get; set; }

    public string DuongDanAnh { get; set; } = string.Empty;

    public bool LaAnhDaiDien { get; set; }

    public int ThuTuHienThi { get; set; }
}
