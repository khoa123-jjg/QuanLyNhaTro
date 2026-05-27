namespace QuanLyNhaTro.Models.TienNghi;

public class TienNghiPhongPageViewModel
{
    public int PhongTroId { get; set; }

    public string MaPhong { get; set; } = string.Empty;

    public string TenPhong { get; set; } = string.Empty;

    public string TenNhaTro { get; set; } = string.Empty;

    public string? TangHienThi { get; set; }

    public string TrangThaiHienThi { get; set; } = string.Empty;

    public List<TienNghiCheckboxViewModel> DanhSachTienNghi { get; set; } = [];
}

public class TienNghiCheckboxViewModel
{
    public int Id { get; set; }

    public string TenTienNghi { get; set; } = string.Empty;

    public bool DaChon { get; set; }
}
