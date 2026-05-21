using QLNhaTro.Models.PhongTro;

namespace QLNhaTro.Models.Home;

public class HomeIndexViewModel
{
    public List<PhongTroCardViewModel> PhongNoiBat { get; set; } = [];

    public int TongPhongTro { get; set; }

    public int TongNguoiDung { get; set; }
}
