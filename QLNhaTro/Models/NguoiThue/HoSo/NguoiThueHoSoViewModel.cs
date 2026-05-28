namespace QLNhaTro.Models.NguoiThue.HoSo;

public class NguoiThueHoSoViewModel
{
    public CapNhatHoSoNguoiThueViewModel ThongTin { get; set; } = new();

    public DoiMatKhauNguoiThueViewModel DoiMatKhau { get; set; } = new();
}
