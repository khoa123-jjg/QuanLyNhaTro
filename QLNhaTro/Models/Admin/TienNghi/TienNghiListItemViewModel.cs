namespace QLNhaTro.Models.Admin.TienNghi;

public class TienNghiListItemViewModel
{
    public int Id { get; set; }

    public string TenTienNghi { get; set; } = string.Empty;

    public string TrangThai { get; set; } = string.Empty;

    public string TenTrangThai { get; set; } = string.Empty;

    public bool DangHienThi { get; set; }
}
