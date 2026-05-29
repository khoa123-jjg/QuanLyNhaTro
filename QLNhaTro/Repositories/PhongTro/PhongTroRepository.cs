using BaiDangEntity = QLNhaTro.Domain.BaiDang;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.PhongTro;

namespace QLNhaTro.Repositories.PhongTro;

public class PhongTroRepository : IPhongTroRepository
{
    private static readonly Dictionary<string, string> KhuVucSlugMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["lien-chieu"] = "Liên Chiểu",
        ["thanh-khe"] = "Thanh Khê",
        ["hai-chau"] = "Hải Châu",
        ["son-tra"] = "Sơn Trà",
        ["ngu-hanh-son"] = "Ngũ Hành Sơn",
        ["cam-le"] = "Cẩm Lệ"
    };

    private readonly PhongTroDaNangContext _context;

    public PhongTroRepository(PhongTroDaNangContext context) => _context = context;

    public async Task<List<PhongTroCardViewModel>> LayPhongNoiBatAsync(int soLuong = 4)
    {
        if (soLuong <= 0) return [];

        var rows = await TaoTruyVanBaiDangHienThi()
            .OrderByDescending(b => b.NgayDuyet ?? b.NgayTao)
            .Take(soLuong)
            .Select(b => new
            {
                b.PhongTroId,
                b.TieuDe,
                b.PhongTro.GiaThueThang,
                b.PhongTro.DienTich,
                b.PhongTro.MoTa,
                b.PhongTro.SoNguoiToiDa,
                b.PhongTro.MaPhong,
                SoNha = b.PhongTro.NhaTro.SoNha,
                DiaChiChiTiet = b.PhongTro.NhaTro.DiaChiChiTiet,
                TenDuong = b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.TenDuong : null,
                TenQuanhuyen = b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.Xa.Quanhuyen.TenQuanhuyen : null,
                TenXa = b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.Xa.TenXahuyen : null,
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                DuongDanAnh = _context.HinhAnhs.Where(h => h.PhongTroId == b.PhongTroId)
                    .OrderByDescending(h => h.LaAnhDaiDien)
                    .ThenBy(h => h.ThuTuHienThi)
                    .Select(h => h.DuongDanAnh)
                    .FirstOrDefault(),
                TienNghi = b.PhongTro.TienNghis.OrderBy(t => t.TenTienNghi).Select(t => t.TenTienNghi).Take(3).ToList()
            })
            .ToListAsync();

        return rows.Select(r => ToCard(r.PhongTroId, r.TieuDe, r.GiaThueThang, r.DienTich, r.MoTa, r.SoNguoiToiDa, r.MaPhong, r.SoNha, r.TenDuong, r.TenXa, r.TenQuanhuyen, r.DiaChiChiTiet, r.TenNhaTro, r.DuongDanAnh, r.TienNghi)).ToList();
    }

    public async Task<PhongTroSearchViewModel> SearchPhongAsync(string? khuVuc, string? mucGia, string? dienTich)
    {
        var query = TaoTruyVanBaiDangHienThi();

        if (!string.IsNullOrWhiteSpace(khuVuc))
        {
            var khuVucLoc = khuVuc.Trim();
            if (KhuVucSlugMap.TryGetValue(khuVucLoc, out var tenQuan))
            {
                query = query.Where(b => b.PhongTro.NhaTro.DuongPho != null && b.PhongTro.NhaTro.DuongPho.Xa.Quanhuyen.TenQuanhuyen == tenQuan);
            }
            else
            {
                query = query.Where(b =>
                    (b.PhongTro.NhaTro.DiaChiChiTiet != null && b.PhongTro.NhaTro.DiaChiChiTiet.Contains(khuVucLoc))
                    || (b.PhongTro.NhaTro.SoNha != null && b.PhongTro.NhaTro.SoNha.Contains(khuVucLoc))
                    || (b.PhongTro.NhaTro.DuongPho != null && (
                        b.PhongTro.NhaTro.DuongPho.TenDuong.Contains(khuVucLoc)
                        || b.PhongTro.NhaTro.DuongPho.Xa.TenXahuyen.Contains(khuVucLoc)
                        || b.PhongTro.NhaTro.DuongPho.Xa.Quanhuyen.TenQuanhuyen.Contains(khuVucLoc)))
                    || b.PhongTro.NhaTro.TenNhaTro.Contains(khuVucLoc));
            }
        }

        if (!string.IsNullOrWhiteSpace(mucGia)) query = ApDungLocMucGia(query, mucGia.Trim());
        if (!string.IsNullOrWhiteSpace(dienTich)) query = ApDungLocDienTich(query, dienTich.Trim());

        var rows = await query
            .OrderByDescending(b => b.NgayDuyet ?? b.NgayTao)
            .Select(b => new
            {
                b.PhongTroId,
                b.TieuDe,
                b.PhongTro.GiaThueThang,
                b.PhongTro.DienTich,
                b.PhongTro.MoTa,
                b.PhongTro.SoNguoiToiDa,
                b.PhongTro.MaPhong,
                SoNha = b.PhongTro.NhaTro.SoNha,
                DiaChiChiTiet = b.PhongTro.NhaTro.DiaChiChiTiet,
                TenDuong = b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.TenDuong : null,
                TenXa = b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.Xa.TenXahuyen : null,
                TenQuanhuyen = b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.Xa.Quanhuyen.TenQuanhuyen : null,
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                DuongDanAnh = _context.HinhAnhs.Where(h => h.PhongTroId == b.PhongTroId)
                    .OrderByDescending(h => h.LaAnhDaiDien)
                    .ThenBy(h => h.ThuTuHienThi)
                    .Select(h => h.DuongDanAnh)
                    .FirstOrDefault(),
                TienNghi = b.PhongTro.TienNghis.OrderBy(t => t.TenTienNghi).Select(t => t.TenTienNghi).Take(3).ToList()
            })
            .ToListAsync();

        return new PhongTroSearchViewModel
        {
            KhuVuc = khuVuc,
            MucGia = mucGia,
            DienTich = dienTich,
            DanhSachPhong = rows.Select(r => ToCard(r.PhongTroId, r.TieuDe, r.GiaThueThang, r.DienTich, r.MoTa, r.SoNguoiToiDa, r.MaPhong, r.SoNha, r.TenDuong, r.TenXa, r.TenQuanhuyen, r.DiaChiChiTiet, r.TenNhaTro, r.DuongDanAnh, r.TienNghi)).ToList()
        };
    }

    public async Task<PhongTroDetailViewModel?> GetChiTietPhongAsync(int id)
    {
        var row = await TaoTruyVanBaiDangHienThi()
            .Where(b => b.PhongTroId == id)
            .OrderByDescending(b => b.NgayDuyet ?? b.NgayTao)
            .Select(b => new
            {
                b.PhongTroId,
                b.TieuDe,
                b.NoiDung,
                b.PhongTro.GiaThueThang,
                b.PhongTro.DienTich,
                b.PhongTro.MoTa,
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                b.PhongTro.NhaTro.DiaChiChiTiet,
                b.PhongTro.NhaTro.SoNha,
                TenDuong = b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.TenDuong : null,
                TenXa = b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.Xa.TenXahuyen : null,
                TenQuanhuyen = b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.Xa.Quanhuyen.TenQuanhuyen : null,
                ViDo = b.PhongTro.NhaTro.ViDo,
                KinhDo = b.PhongTro.NhaTro.KinhDo,
                TienNghi = b.PhongTro.TienNghis.OrderBy(t => t.TenTienNghi).Select(t => t.TenTienNghi).ToList(),
                SoDienThoai = b.PhongTro.NhaTro.ChuNhaTro.NguoiDung.SoDienThoai
            })
            .FirstOrDefaultAsync();

        if (row is null) return null;

        var danhSachAnh = await _context.HinhAnhs
            .AsNoTracking()
            .Where(h => h.PhongTroId == id)
            .Where(h => h.DuongDanAnh != null && h.DuongDanAnh != "")
            .OrderByDescending(h => h.LaAnhDaiDien)
            .ThenBy(h => h.ThuTuHienThi)
            .Select(h => TaoDuongDanAnh(h.DuongDanAnh))
            .ToListAsync();
        if (danhSachAnh.Count == 0) danhSachAnh.Add(DefaultImage.PhongTro);

        var moTa = !string.IsNullOrWhiteSpace(row.MoTa) ? row.MoTa.Trim() : row.NoiDung.Trim();

        return new PhongTroDetailViewModel
        {
            PhongTroId = row.PhongTroId,
            TieuDe = row.TieuDe,
            GiaThue = row.GiaThueThang,
            DienTich = row.DienTich,
            DiaChi = TaoDiaChi(row.TenDuong, row.SoNha, row.TenXa, row.TenQuanhuyen, row.DiaChiChiTiet),
            MoTa = moTa,
            DanhSachAnh = danhSachAnh,
            TienNghi = row.TienNghi,
            TenNhaTro = row.TenNhaTro,
            SoDienThoaiLienHe = row.SoDienThoai?.Trim() ?? string.Empty,
            ViDo = row.ViDo,
            KinhDo = row.KinhDo
        };
    }

    public async Task<PhongTroMapPageViewModel> GetBanDoPhongAsync(int? phongTroId = null)
    {
        var query = TaoTruyVanBaiDangHienThi()
            .Where(b => b.PhongTro.NhaTro.ViDo != null && b.PhongTro.NhaTro.KinhDo != null);

        if (phongTroId is > 0)
        {
            query = query.Where(b => b.PhongTroId == phongTroId.Value);
        }

        var rows = await query
            .Select(b => new PhongTroMapItemViewModel
            {
                Id = b.PhongTroId,
                TieuDe = b.TieuDe,
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                DiaChi = TaoDiaChi(b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.TenDuong : null, b.PhongTro.NhaTro.SoNha, b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.Xa.TenXahuyen : null, b.PhongTro.NhaTro.DuongPho != null ? b.PhongTro.NhaTro.DuongPho.Xa.Quanhuyen.TenQuanhuyen : null, b.PhongTro.NhaTro.DiaChiChiTiet),
                GiaThue = b.PhongTro.GiaThueThang,
                DienTich = b.PhongTro.DienTich,
                ViDo = b.PhongTro.NhaTro.ViDo ?? 0,
                KinhDo = b.PhongTro.NhaTro.KinhDo ?? 0,
                AnhDaiDien = _context.HinhAnhs.Where(h => h.PhongTroId == b.PhongTroId)
                    .OrderByDescending(h => h.LaAnhDaiDien)
                    .ThenBy(h => h.ThuTuHienThi)
                    .Select(h => h.DuongDanAnh)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return new PhongTroMapPageViewModel
        {
            PhongTros = rows,
            FocusPhongTroId = phongTroId
        };
    }

    private static string TaoDiaChi(string? tenDuong, string? soNha, string? tenXa, string? tenQuanhuyen, string diaChiChiTiet)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(soNha)) parts.Add(soNha.Trim());
        if (!string.IsNullOrWhiteSpace(tenDuong)) parts.Add(tenDuong.Trim());
        if (!string.IsNullOrWhiteSpace(tenXa)) parts.Add(tenXa.Trim());
        if (!string.IsNullOrWhiteSpace(tenQuanhuyen)) parts.Add(tenQuanhuyen.Trim());
        parts.Add("Đà Nẵng");
        return parts.Count > 1 ? string.Join(", ", parts) : string.IsNullOrWhiteSpace(diaChiChiTiet) ? "Đà Nẵng" : diaChiChiTiet.Trim();
    }

    private IQueryable<BaiDangEntity> TaoTruyVanBaiDangHienThi() => _context.BaiDangs.AsNoTracking()
        .Where(b => b.TrangThaiDuyet == BaiDangStatus.DaDuyet)
        .Where(b => b.PhongTro.TrangThai != "TAM_AN")
        .Where(b => b.PhongTro.NhaTro.TrangThai == "HOAT_DONG");

    private static IQueryable<BaiDangEntity> ApDungLocMucGia(IQueryable<BaiDangEntity> query, string mucGia) => mucGia switch
    {
        "0-2000000" => query.Where(b => b.PhongTro.GiaThueThang < 2_000_000),
        "2000000-3000000" => query.Where(b => b.PhongTro.GiaThueThang >= 2_000_000 && b.PhongTro.GiaThueThang <= 3_000_000),
        "3000000-5000000" => query.Where(b => b.PhongTro.GiaThueThang >= 3_000_000 && b.PhongTro.GiaThueThang <= 5_000_000),
        "5000000+" => query.Where(b => b.PhongTro.GiaThueThang >= 5_000_000),
        _ => query
    };

    private static IQueryable<BaiDangEntity> ApDungLocDienTich(IQueryable<BaiDangEntity> query, string dienTich) => dienTich switch
    {
        "0-15" => query.Where(b => b.PhongTro.DienTich < 15),
        "15-25" => query.Where(b => b.PhongTro.DienTich >= 15 && b.PhongTro.DienTich <= 25),
        "25-40" => query.Where(b => b.PhongTro.DienTich >= 25 && b.PhongTro.DienTich <= 40),
        "40+" => query.Where(b => b.PhongTro.DienTich > 40),
        _ => query
    };

    private static PhongTroCardViewModel ToCard(int phongTroId, string tieuDe, decimal giaThueThang, decimal dienTich, string? moTa, int? soNguoiToiDa, string maPhong, string? soNha, string? tenDuong, string? tenXa, string? tenQuanhuyen, string? diaChiChiTiet, string tenNhaTro, string? duongDanAnh, List<string> tienNghi) => new()
    {
        Id = phongTroId,
        TieuDe = tieuDe,
        KhuVuc = TaoDiaChiCard(soNha, tenDuong, tenXa, tenQuanhuyen, diaChiChiTiet, tenNhaTro),
        GiaThue = giaThueThang,
        DienTich = dienTich,
        ThongTinPhu = TaoThongTinPhu(tienNghi, moTa, soNguoiToiDa, maPhong),
        AnhDaiDien = TaoDuongDanAnh(duongDanAnh)
    };

    private static string TaoDiaChiCard(string? soNha, string? tenDuong, string? tenXa, string? tenQuanhuyen, string? diaChiChiTiet, string tenNhaTro)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(soNha)) parts.Add(soNha.Trim());
        if (!string.IsNullOrWhiteSpace(tenDuong)) parts.Add(tenDuong.Trim());
        if (!string.IsNullOrWhiteSpace(tenXa)) parts.Add(tenXa.Trim());
        if (!string.IsNullOrWhiteSpace(tenQuanhuyen)) parts.Add(tenQuanhuyen.Trim());
        if (parts.Count > 0) return string.Join(", ", parts);
        if (!string.IsNullOrWhiteSpace(diaChiChiTiet)) return diaChiChiTiet.Trim();
        return tenNhaTro.Trim();
    }

    private static string TaoThongTinPhu(IReadOnlyList<string> tienNghi, string? moTa, int? soNguoiToiDa, string maPhong)
    {
        if (tienNghi.Count > 0) return string.Join(" · ", tienNghi);
        if (!string.IsNullOrWhiteSpace(moTa)) { var text = moTa.Trim(); return text.Length <= 60 ? text : text[..60] + "…"; }
        if (soNguoiToiDa is > 0) return $"Tối đa {soNguoiToiDa} người";
        return maPhong.Trim();
    }

    private static string TaoDuongDanAnh(string? duongDanAnh)
    {
        if (string.IsNullOrWhiteSpace(duongDanAnh)) return DefaultImage.PhongTro;
        var path = duongDanAnh.Trim();
        return path.StartsWith('/') || path.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? path : "/" + path.TrimStart('/');
    }
}
