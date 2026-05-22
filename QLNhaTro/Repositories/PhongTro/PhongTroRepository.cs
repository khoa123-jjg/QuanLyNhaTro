using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Domain;
using QLNhaTro.Models.PhongTro;
using QuanLyNhaTro.Helpers.Constants;

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

    public PhongTroRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<List<PhongTroCardViewModel>> LayPhongNoiBatAsync(int soLuong = 4)
    {
        if (soLuong <= 0)
        {
            return [];
        }

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
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                TenQuanhuyen = b.PhongTro.NhaTro.DuongPho != null
                    ? b.PhongTro.NhaTro.DuongPho.Xa.Quanhuyen.TenQuanhuyen
                    : null,
                TenXa = b.PhongTro.NhaTro.DuongPho != null
                    ? b.PhongTro.NhaTro.DuongPho.Xa.TenXahuyen
                    : null,
                DuongDanAnh = b.PhongTro.HinhAnh != null
                    ? b.PhongTro.HinhAnh.DuongDanAnh
                    : null,
                TienNghi = b.PhongTro.TienNghis
                    .OrderBy(t => t.TenTienNghi)
                    .Select(t => t.TenTienNghi)
                    .Take(3)
                    .ToList()
            })
            .ToListAsync();

        return rows.Select(r => ToCard(
            r.PhongTroId,
            r.TieuDe,
            r.GiaThueThang,
            r.DienTich,
            r.MoTa,
            r.SoNguoiToiDa,
            r.MaPhong,
            r.TenNhaTro,
            r.TenQuanhuyen,
            r.TenXa,
            r.DuongDanAnh,
            r.TienNghi)).ToList();
    }

    public async Task<PhongTroSearchViewModel> SearchPhongAsync(string? khuVuc, string? mucGia, string? dienTich)
    {
        var query = TaoTruyVanBaiDangHienThi();

        if (!string.IsNullOrWhiteSpace(khuVuc) && KhuVucSlugMap.TryGetValue(khuVuc.Trim(), out var tenQuan))
        {
            query = query.Where(b =>
                b.PhongTro.NhaTro.DuongPho != null
                && b.PhongTro.NhaTro.DuongPho.Xa.Quanhuyen.TenQuanhuyen == tenQuan);
        }

        if (!string.IsNullOrWhiteSpace(mucGia))
        {
            query = ApDungLocMucGia(query, mucGia.Trim());
        }

        if (!string.IsNullOrWhiteSpace(dienTich))
        {
            query = ApDungLocDienTich(query, dienTich.Trim());
        }

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
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                TenQuanhuyen = b.PhongTro.NhaTro.DuongPho != null
                    ? b.PhongTro.NhaTro.DuongPho.Xa.Quanhuyen.TenQuanhuyen
                    : null,
                TenXa = b.PhongTro.NhaTro.DuongPho != null
                    ? b.PhongTro.NhaTro.DuongPho.Xa.TenXahuyen
                    : null,
                DuongDanAnh = b.PhongTro.HinhAnh != null
                    ? b.PhongTro.HinhAnh.DuongDanAnh
                    : null,
                TienNghi = b.PhongTro.TienNghis
                    .OrderBy(t => t.TenTienNghi)
                    .Select(t => t.TenTienNghi)
                    .Take(3)
                    .ToList()
            })
            .ToListAsync();

        return new PhongTroSearchViewModel
        {
            KhuVuc = khuVuc,
            MucGia = mucGia,
            DienTich = dienTich,
            DanhSachPhong = rows.Select(r => ToCard(
                r.PhongTroId,
                r.TieuDe,
                r.GiaThueThang,
                r.DienTich,
                r.MoTa,
                r.SoNguoiToiDa,
                r.MaPhong,
                r.TenNhaTro,
                r.TenQuanhuyen,
                r.TenXa,
                r.DuongDanAnh,
                r.TienNghi)).ToList()
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
                TenDuong = b.PhongTro.NhaTro.DuongPho != null
                    ? b.PhongTro.NhaTro.DuongPho.TenDuong
                    : null,
                TenXa = b.PhongTro.NhaTro.DuongPho != null
                    ? b.PhongTro.NhaTro.DuongPho.Xa.TenXahuyen
                    : null,
                TenQuanhuyen = b.PhongTro.NhaTro.DuongPho != null
                    ? b.PhongTro.NhaTro.DuongPho.Xa.Quanhuyen.TenQuanhuyen
                    : null,
                DuongDanAnh = b.PhongTro.HinhAnh != null
                    ? b.PhongTro.HinhAnh.DuongDanAnh
                    : null,
                TienNghi = b.PhongTro.TienNghis
                    .OrderBy(t => t.TenTienNghi)
                    .Select(t => t.TenTienNghi)
                    .ToList(),
                SoDienThoai = b.PhongTro.NhaTro.ChuNhaTro.NguoiDung.SoDienThoai
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        var danhSachAnh = new List<string>();
        if (!string.IsNullOrWhiteSpace(row.DuongDanAnh))
        {
            danhSachAnh.Add(TaoDuongDanAnh(row.DuongDanAnh));
        }

        if (danhSachAnh.Count == 0)
        {
            danhSachAnh.Add(DefaultImage.PhongTro);
        }

        var moTa = !string.IsNullOrWhiteSpace(row.MoTa)
            ? row.MoTa.Trim()
            : row.NoiDung.Trim();

        return new PhongTroDetailViewModel
        {
            Id = row.PhongTroId,
            TieuDe = row.TieuDe,
            GiaThue = row.GiaThueThang,
            DienTich = row.DienTich,
            DiaChi = TaoDiaChi(row.TenDuong, row.SoNha, row.TenXa, row.TenQuanhuyen, row.DiaChiChiTiet),
            MoTa = moTa,
            DanhSachAnh = danhSachAnh,
            TienNghi = row.TienNghi,
            TenNhaTro = row.TenNhaTro,
            SoDienThoaiLienHe = row.SoDienThoai?.Trim() ?? string.Empty
        };
    }

    private static string TaoDiaChi(
        string? tenDuong,
        string? soNha,
        string? tenXa,
        string? tenQuanhuyen,
        string diaChiChiTiet)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(soNha))
        {
            parts.Add(soNha.Trim());
        }

        if (!string.IsNullOrWhiteSpace(tenDuong))
        {
            parts.Add(tenDuong.Trim());
        }

        if (!string.IsNullOrWhiteSpace(tenXa))
        {
            parts.Add(tenXa.Trim());
        }

        if (!string.IsNullOrWhiteSpace(tenQuanhuyen))
        {
            parts.Add(tenQuanhuyen.Trim());
        }

        parts.Add("Đà Nẵng");

        if (parts.Count > 1)
        {
            return string.Join(", ", parts);
        }

        return string.IsNullOrWhiteSpace(diaChiChiTiet)
            ? "Đà Nẵng"
            : diaChiChiTiet.Trim();
    }

    private IQueryable<BaiDang> TaoTruyVanBaiDangHienThi()
    {
        return _context.BaiDangs
            .AsNoTracking()
            .Where(b => b.TrangThaiDuyet == BaiDangStatus.DaDuyet)
            .Where(b => b.PhongTro.TrangThai != PhongTroStatus.TamAn)
            .Where(b => b.PhongTro.NhaTro.TrangThai == NhaTroStatus.HoatDong);
    }

    private static IQueryable<BaiDang> ApDungLocMucGia(IQueryable<BaiDang> query, string mucGia) =>
        mucGia switch
        {
            "0-2000000" => query.Where(b => b.PhongTro.GiaThueThang < 2_000_000),
            "2000000-3000000" => query.Where(b =>
                b.PhongTro.GiaThueThang >= 2_000_000 && b.PhongTro.GiaThueThang <= 3_000_000),
            "3000000-5000000" => query.Where(b =>
                b.PhongTro.GiaThueThang >= 3_000_000 && b.PhongTro.GiaThueThang <= 5_000_000),
            "5000000+" => query.Where(b => b.PhongTro.GiaThueThang >= 5_000_000),
            _ => query
        };

    private static IQueryable<BaiDang> ApDungLocDienTich(IQueryable<BaiDang> query, string dienTich) =>
        dienTich switch
        {
            "0-15" => query.Where(b => b.PhongTro.DienTich < 15),
            "15-25" => query.Where(b => b.PhongTro.DienTich >= 15 && b.PhongTro.DienTich <= 25),
            "25-40" => query.Where(b => b.PhongTro.DienTich >= 25 && b.PhongTro.DienTich <= 40),
            "40+" => query.Where(b => b.PhongTro.DienTich > 40),
            _ => query
        };

    private static PhongTroCardViewModel ToCard(
        int phongTroId,
        string tieuDe,
        decimal giaThueThang,
        decimal dienTich,
        string? moTa,
        int? soNguoiToiDa,
        string maPhong,
        string tenNhaTro,
        string? tenQuanhuyen,
        string? tenXa,
        string? duongDanAnh,
        List<string> tienNghi) =>
        new()
        {
            Id = phongTroId,
            TieuDe = tieuDe,
            KhuVuc = TaoKhuVuc(tenXa, tenQuanhuyen, tenNhaTro),
            GiaThue = giaThueThang,
            DienTich = dienTich,
            ThongTinPhu = TaoThongTinPhu(tienNghi, moTa, soNguoiToiDa, maPhong),
            AnhDaiDien = TaoDuongDanAnh(duongDanAnh)
        };

    private static string TaoKhuVuc(string? tenXa, string? tenQuanhuyen, string tenNhaTro)
    {
        if (!string.IsNullOrWhiteSpace(tenXa) && !string.IsNullOrWhiteSpace(tenQuanhuyen))
        {
            return $"{tenXa.Trim()}, {tenQuanhuyen.Trim()}";
        }

        if (!string.IsNullOrWhiteSpace(tenQuanhuyen))
        {
            return tenQuanhuyen.Trim();
        }

        return tenNhaTro.Trim();
    }

    private static string TaoThongTinPhu(
        IReadOnlyList<string> tienNghi,
        string? moTa,
        int? soNguoiToiDa,
        string maPhong)
    {
        if (tienNghi.Count > 0)
        {
            return string.Join(" · ", tienNghi);
        }

        if (!string.IsNullOrWhiteSpace(moTa))
        {
            var text = moTa.Trim();
            return text.Length <= 60 ? text : text[..60] + "…";
        }

        if (soNguoiToiDa is > 0)
        {
            return $"Tối đa {soNguoiToiDa} người";
        }

        return maPhong.Trim();
    }

    private static string TaoDuongDanAnh(string? duongDanAnh)
    {
        if (string.IsNullOrWhiteSpace(duongDanAnh))
        {
            return DefaultImage.PhongTro;
        }

        var path = duongDanAnh.Trim();
        return path.StartsWith('/') || path.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? path
            : "/" + path.TrimStart('/');
    }
}
