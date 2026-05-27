using System;
using System.Collections.Generic;

namespace QLNhaTroV7.Domain;

public partial class DuongPho
{
    public int Id { get; set; }

    public int DonViHanhChinhId { get; set; }

    public string TenDuong { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual DonViHanhChinh DonViHanhChinh { get; set; } = null!;

    public virtual ICollection<NhaTro> NhaTros { get; set; } = new List<NhaTro>();
}
