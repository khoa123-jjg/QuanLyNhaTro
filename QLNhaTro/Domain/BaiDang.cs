using System;
using System.Collections.Generic;

namespace QLNhaTro.Domain;

public partial class BaiDang
{
    public int Id { get; set; }

    public int PhongTroId { get; set; }

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public string TrangThaiDuyet { get; set; } = null!;

    public string? LyDoTuChoi { get; set; }

    public int? NguoiDuyetId { get; set; }

    public DateTime? NgayGuiDuyet { get; set; }

    public DateTime? NgayDuyet { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual NguoiDung? NguoiDuyet { get; set; }

    public virtual PhongTro PhongTro { get; set; } = null!;
}
