namespace QLNhaTro.Models.Admin.Dashboard;

public class AdminDashboardViewModel
{
    public int TongNguoiDung { get; set; }

    public int NguoiDungMoiTrong7Ngay { get; set; }

    public int BaiChoDuyet { get; set; }

    public int BaiChoDuyetMoiTrong7Ngay { get; set; }

    public int TongNhaTro { get; set; }

    public int NhaTroMoiTrong7Ngay { get; set; }

    public int TongTienNghi { get; set; }

    public int TienNghiMoiTrong7Ngay { get; set; }

    public List<AdminDashboardBaiChoDuyetItemViewModel> BaiChoDuyetGanDay { get; set; } = new();
}
