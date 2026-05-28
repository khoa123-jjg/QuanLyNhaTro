using System.Collections.Generic;

namespace QLNhaTro.Models.Admin.TienNghi;

public class TienNghiPageViewModel
{
    public List<TienNghiListItemViewModel> DanhSachTienNghi { get; set; } = [];

    public TienNghiFormViewModel Form { get; set; } = new();

    public int TongSoTienNghi { get; set; }
}
