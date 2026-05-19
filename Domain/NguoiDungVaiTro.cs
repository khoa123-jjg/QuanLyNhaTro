using System;
using System.Collections.Generic;

namespace QuanLyNhaTro.Domain;

public partial class NguoiDungVaiTro
{
    public int NguoiDungId { get; set; }

    public int VaiTroId { get; set; }

    public DateTime NgayGan { get; set; }

    public virtual NguoiDung NguoiDung { get; set; } = null!;

    public virtual VaiTro VaiTro { get; set; } = null!;
}
