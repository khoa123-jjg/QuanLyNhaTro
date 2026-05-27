using System;
using System.Collections.Generic;

namespace QLNhaTroV7.Domain;

public partial class DatThue
{
    public int Id { get; set; }

    public int NguoiThueId { get; set; }

    public int PhongTroId { get; set; }

    public string HoTenLienHe { get; set; } = null!;

    public string SoDienThoaiLienHe { get; set; } = null!;

    public string? LoiNhan { get; set; }

    public DateTime? NgayMuonXemPhong { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? LyDoTuChoi { get; set; }

    public string? GhiChuChuTro { get; set; }

    public DateTime? NgayXuLy { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual NguoiThue NguoiThue { get; set; } = null!;

    public virtual PhongTro PhongTro { get; set; } = null!;
}
