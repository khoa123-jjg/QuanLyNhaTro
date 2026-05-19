using System;
using System.Collections.Generic;

namespace QuanLyNhaTro.Domain;

public partial class BaiDang
{
    public int Id { get; set; }

    public int NhaTroId { get; set; }

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

    public virtual NhaTro NhaTro { get; set; } = null!;

    public virtual ICollection<LoaiPhong> LoaiPhongs { get; set; } = new List<LoaiPhong>();
}
