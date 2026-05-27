using System;
using System.Collections.Generic;

namespace QLNhaTroV7.Domain;

public partial class ChuNhaTro
{
    public int Id { get; set; }

    public int NguoiDungId { get; set; }

    public string TrangThaiHoSo { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual NguoiDung NguoiDung { get; set; } = null!;

    public virtual ICollection<NhaTro> NhaTros { get; set; } = new List<NhaTro>();
}
