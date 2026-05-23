using BaiDangEntity = QLNhaTro.Domain.BaiDang;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.BaiDang;

namespace QLNhaTro.Repositories.BaiDang;

public class BaiDangRepository : IBaiDangRepository
{
    private readonly PhongTroDaNangContext _context;

    public BaiDangRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<BaiDangListPageViewModel> GetDanhSachBaiDangAsync(
        string userId,
        string? tuKhoa,
        int? nhaTroId,
        string? trangThaiDuyet)
    {
        var page = new BaiDangListPageViewModel
        {
            TuKhoa = tuKhoa,
            NhaTroId = nhaTroId,
            TrangThaiDuyet = trangThaiDuyet
        };

        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return page;
        }

        page.DanhSachNhaTro = await LayDanhSachNhaTroAsync(nguoiDungId);
        page.DanhSachTrangThai = TaoDanhSachTrangThai();

        var query = _context.BaiDangs
            .AsNoTracking()
            .Where(b => b.PhongTro.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId);

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(b =>
                b.TieuDe.Contains(keyword) ||
                b.NoiDung.Contains(keyword) ||
                b.PhongTro.MaPhong.Contains(keyword));
        }

        if (nhaTroId is > 0)
        {
            query = query.Where(b => b.PhongTro.NhaTroId == nhaTroId.Value);
        }

        if (!string.IsNullOrWhiteSpace(trangThaiDuyet) && BaiDangStatus.IsValid(trangThaiDuyet))
        {
            var trangThai = trangThaiDuyet.Trim();
            query = query.Where(b => b.TrangThaiDuyet == trangThai);
        }

        page.DanhSachBaiDang = await query
            .OrderByDescending(b => b.NgayTao)
            .Select(b => new BaiDangListItemViewModel
            {
                Id = b.Id,
                TieuDe = b.TieuDe,
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                MaPhong = b.PhongTro.MaPhong,
                AnhDaiDien = b.PhongTro.HinhAnh != null
                    ? b.PhongTro.HinhAnh.DuongDanAnh
                    : null,
                GiaThueThang = b.PhongTro.GiaThueThang,
                DienTich = b.PhongTro.DienTich,
                SoNguoiToiDa = b.PhongTro.SoNguoiToiDa,
                TrangThaiDuyet = b.TrangThaiDuyet,
                NgayTao = b.NgayTao
            })
            .ToListAsync();

        foreach (var item in page.DanhSachBaiDang)
        {
            item.AnhDaiDien = TaoDuongDanAnh(item.AnhDaiDien);
        }

        return page;
    }

    public async Task<BaiDangCreateUpdateViewModel> GetCreateModelAsync(string userId, int? nhaTroId = null)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return new BaiDangCreateUpdateViewModel();
        }

        return new BaiDangCreateUpdateViewModel
        {
            NhaTroId = nhaTroId,
            DanhSachNhaTro = await LayDanhSachNhaTroAsync(nguoiDungId),
            DanhSachPhong = await LayDanhSachPhongAsync(nguoiDungId, nhaTroId)
        };
    }

    public async Task<BaiDangCreateUpdateViewModel?> GetUpdateModelAsync(string userId, int baiDangId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return null;
        }

        var row = await _context.BaiDangs
            .AsNoTracking()
            .Where(b => b.Id == baiDangId)
            .Where(b => b.TrangThaiDuyet == BaiDangStatus.Nhap)
            .Where(b => b.PhongTro.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Select(b => new
            {
                b.Id,
                b.TieuDe,
                b.NoiDung,
                b.PhongTroId,
                b.TrangThaiDuyet,
                b.PhongTro.NhaTroId
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        return new BaiDangCreateUpdateViewModel
        {
            Id = row.Id,
            NhaTroId = row.NhaTroId,
            PhongTroId = row.PhongTroId,
            TieuDe = row.TieuDe,
            NoiDung = row.NoiDung,
            TrangThaiDuyet = row.TrangThaiDuyet,
            DanhSachNhaTro = await LayDanhSachNhaTroAsync(nguoiDungId),
            DanhSachPhong = await LayDanhSachPhongAsync(nguoiDungId, row.NhaTroId)
        };
    }

    public async Task<PhongDangBaiViewModel?> GetThongTinPhongAsync(string userId, int phongTroId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId) || phongTroId <= 0)
        {
            return null;
        }

        var row = await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.Id == phongTroId)
            .Where(p => p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Select(p => new PhongDangBaiViewModel
            {
                Id = p.Id,
                MaPhong = p.MaPhong,
                TenNhaTro = p.NhaTro.TenNhaTro,
                GiaThueThang = p.GiaThueThang,
                DienTich = p.DienTich,
                SoNguoiToiDa = p.SoNguoiToiDa,
                AnhDaiDien = p.HinhAnh != null ? p.HinhAnh.DuongDanAnh : null
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        row.AnhDaiDien = TaoDuongDanAnh(row.AnhDaiDien);
        return row;
    }

    public async Task<bool> LuuNhapAsync(string userId, BaiDangCreateUpdateViewModel model)
    {
        var id = await LuuNhapNoiDungAsync(userId, model);
        return id.HasValue;
    }

    public Task<int?> LuuNhapVaTraVeIdAsync(string userId, BaiDangCreateUpdateViewModel model) =>
        LuuNhapNoiDungAsync(userId, model);

    public async Task<GuiBaiChoDuyetViewModel?> GetGuiBaiChoDuyetAsync(string userId, int baiDangId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return null;
        }

        var row = await _context.BaiDangs
            .AsNoTracking()
            .Where(b => b.Id == baiDangId)
            .Where(b => b.TrangThaiDuyet == BaiDangStatus.Nhap)
            .Where(b => b.PhongTro.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Select(b => new
            {
                b.Id,
                b.TieuDe,
                b.NoiDung,
                b.TrangThaiDuyet,
                b.PhongTro.MaPhong,
                b.PhongTro.GiaThueThang,
                b.PhongTro.DienTich,
                b.PhongTro.SoNguoiToiDa,
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
                SoDienThoai = b.PhongTro.NhaTro.ChuNhaTro.NguoiDung.SoDienThoai,
                TienNghi = b.PhongTro.TienNghis
                    .OrderBy(t => t.TenTienNghi)
                    .Select(t => t.TenTienNghi)
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        return new GuiBaiChoDuyetViewModel
        {
            Id = row.Id,
            TieuDe = row.TieuDe,
            NoiDung = row.NoiDung,
            TrangThaiDuyet = row.TrangThaiDuyet,
            TenNhaTro = row.TenNhaTro,
            MaPhong = row.MaPhong,
            AnhDaiDien = TaoDuongDanAnh(row.DuongDanAnh),
            GiaThueThang = row.GiaThueThang,
            DienTich = row.DienTich,
            SoNguoiToiDa = row.SoNguoiToiDa,
            DiaChi = TaoDiaChi(row.TenDuong, row.SoNha, row.TenXa, row.TenQuanhuyen, row.DiaChiChiTiet),
            SoDienThoaiLienHe = row.SoDienThoai?.Trim(),
            TienNghi = row.TienNghi
        };
    }

    public async Task<bool> XacNhanGuiChoDuyetAsync(string userId, int baiDangId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return false;
        }

        var baiDang = await _context.BaiDangs
            .Where(b => b.Id == baiDangId)
            .Where(b => b.TrangThaiDuyet == BaiDangStatus.Nhap)
            .Where(b => b.PhongTro.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .FirstOrDefaultAsync();

        if (baiDang is null)
        {
            return false;
        }

        baiDang.TrangThaiDuyet = BaiDangStatus.ChoDuyet;
        baiDang.NgayCapNhat = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<int?> LuuNhapNoiDungAsync(string userId, BaiDangCreateUpdateViewModel model)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return null;
        }

        if (model.PhongTroId is not > 0)
        {
            return null;
        }

        var phongThuocChuTro = await _context.PhongTros
            .AnyAsync(p =>
                p.Id == model.PhongTroId.Value &&
                p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId);

        if (!phongThuocChuTro)
        {
            return null;
        }

        var tieuDe = model.TieuDe.Trim();
        var noiDung = model.NoiDung.Trim();

        if (model.Id is > 0)
        {
            var baiDang = await _context.BaiDangs
                .Where(b => b.Id == model.Id.Value)
                .Where(b => b.TrangThaiDuyet == BaiDangStatus.Nhap)
                .Where(b => b.PhongTro.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
                .FirstOrDefaultAsync();

            if (baiDang is null)
            {
                return null;
            }

            baiDang.TieuDe = tieuDe;
            baiDang.NoiDung = noiDung;
            baiDang.PhongTroId = model.PhongTroId.Value;
            baiDang.TrangThaiDuyet = BaiDangStatus.Nhap;
            baiDang.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();
            return baiDang.Id;
        }

        var moi = new BaiDangEntity
        {
            PhongTroId = model.PhongTroId.Value,
            TieuDe = tieuDe,
            NoiDung = noiDung,
            TrangThaiDuyet = BaiDangStatus.Nhap,
            NgayTao = DateTime.Now
        };
        _context.BaiDangs.Add(moi);
        await _context.SaveChangesAsync();
        return moi.Id;
    }

    private async Task<List<SelectListItem>> LayDanhSachNhaTroAsync(int nguoiDungId) =>
        await _context.NhaTros
            .AsNoTracking()
            .Where(n => n.ChuNhaTro.NguoiDungId == nguoiDungId)
            .OrderBy(n => n.TenNhaTro)
            .Select(n => new SelectListItem(n.TenNhaTro, n.Id.ToString()))
            .ToListAsync();

    private async Task<List<PhongDangBaiViewModel>> LayDanhSachPhongAsync(int nguoiDungId, int? nhaTroId)
    {
        var query = _context.PhongTros
            .AsNoTracking()
            .Where(p => p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId);

        if (nhaTroId is > 0)
        {
            query = query.Where(p => p.NhaTroId == nhaTroId.Value);
        }

        var rows = await query
            .OrderBy(p => p.NhaTro.TenNhaTro)
            .ThenBy(p => p.MaPhong)
            .Select(p => new PhongDangBaiViewModel
            {
                Id = p.Id,
                MaPhong = p.MaPhong,
                TenNhaTro = p.NhaTro.TenNhaTro,
                GiaThueThang = p.GiaThueThang,
                DienTich = p.DienTich,
                SoNguoiToiDa = p.SoNguoiToiDa,
                AnhDaiDien = p.HinhAnh != null ? p.HinhAnh.DuongDanAnh : null
            })
            .ToListAsync();

        foreach (var phong in rows)
        {
            phong.AnhDaiDien = TaoDuongDanAnh(phong.AnhDaiDien);
        }

        return rows;
    }

    private static List<SelectListItem> TaoDanhSachTrangThai() =>
    [
        new SelectListItem("Tất cả trạng thái", ""),
        new SelectListItem(BaiDangStatus.GetDisplayName(BaiDangStatus.Nhap), BaiDangStatus.Nhap),
        new SelectListItem(BaiDangStatus.GetDisplayName(BaiDangStatus.ChoDuyet), BaiDangStatus.ChoDuyet),
        new SelectListItem(BaiDangStatus.GetDisplayName(BaiDangStatus.DaDuyet), BaiDangStatus.DaDuyet),
        new SelectListItem(BaiDangStatus.GetDisplayName(BaiDangStatus.BiTuChoi), BaiDangStatus.BiTuChoi)
    ];

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

    private static bool TryParseNguoiDungId(string userId, out int nguoiDungId) =>
        int.TryParse(userId, out nguoiDungId) && nguoiDungId > 0;
}
