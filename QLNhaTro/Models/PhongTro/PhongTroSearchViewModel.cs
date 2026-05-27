namespace QLNhaTro.Models.PhongTro;

public class PhongTroSearchViewModel
{
    public string? KhuVuc { get; set; }

    public string? MucGia { get; set; }

    public string? DienTich { get; set; }

    public List<PhongTroCardViewModel> DanhSachPhong { get; set; } = [];
}
