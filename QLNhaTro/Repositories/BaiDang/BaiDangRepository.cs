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
        //Trả về danh sách nhà trọ nếu có điều kiện tìm kiếm
        string userId,
        string? tuKhoa,
        int? nhaTroId,
        string? trangThaiDuyet)
    {
        //Gán điều kiện tìm kiếm
        var page = new BaiDangListPageViewModel
        {
            TuKhoa = tuKhoa,
            NhaTroId = nhaTroId,
            TrangThaiDuyet = trangThaiDuyet
        };
        // Ep kiểu id người dùng
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return page;
        }
        // Lấy danh sách để dổ và dropdown (nhà trọ, trạng thái)
        page.DanhSachNhaTro = await LayDanhSachNhaTroAsync(nguoiDungId);
        page.DanhSachTrangThai = TaoDanhSachTrangThai();
        // Truy vấn theo điều kiện nếu có
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
        var rows = await query
            .OrderByDescending(b => b.NgayTao)
            .Select(b => new
            {
                b.Id,
                b.TieuDe,
                b.PhongTroId,
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                MaPhong = b.PhongTro.MaPhong,
                GiaThueThang = b.PhongTro.GiaThueThang,
                DienTich = b.PhongTro.DienTich,
                SoNguoiToiDa = b.PhongTro.SoNguoiToiDa,
                b.TrangThaiDuyet,
                b.NgayTao
            })
            .ToListAsync();

        if (rows.Count == 0)
        {
            return page;
        }

        var phongTroIds = rows
            .Select(x => x.PhongTroId)
            .Distinct()
            .ToList();

        var anhTheoPhong = await _context.HinhAnhs
            .AsNoTracking()
            .Where(h => phongTroIds.Contains(h.PhongTroId))
            .OrderByDescending(h => h.LaAnhDaiDien)
            .ThenBy(h => h.ThuTuHienThi)
            .Select(h => new
            {
                h.PhongTroId,
                h.DuongDanAnh
            })
            .ToListAsync();

        var anhDaiDienTheoPhong = anhTheoPhong
            .GroupBy(h => h.PhongTroId)
            .ToDictionary(
                g => g.Key,
                g => g.First().DuongDanAnh
            );

        page.DanhSachBaiDang = rows
            .Select(row =>
            {
                anhDaiDienTheoPhong.TryGetValue(row.PhongTroId, out var duongDanAnh);

                return new BaiDangListItemViewModel
                {
                    Id = row.Id,
                    TieuDe = row.TieuDe,
                    TenNhaTro = row.TenNhaTro,
                    MaPhong = row.MaPhong,
                    AnhDaiDien = TaoDuongDanAnh(duongDanAnh),
                    GiaThueThang = row.GiaThueThang,
                    DienTich = row.DienTich,
                    SoNguoiToiDa = row.SoNguoiToiDa,
                    TrangThaiDuyet = row.TrangThaiDuyet,
                    NgayTao = row.NgayTao
                };
            })
            .ToList();

        return page;
    }
    // Các dữ liệu cần cho giao diện tạo bài đăng
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
    // Các dữ liệu cần  cho giao diện update, đã có id phòng trọ
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
        // Cả 2 tạo mới và update đều trả về modle BaiDangCreateUpdateViewModel
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
                AnhDaiDien = _context.HinhAnhs
                    .Where(h => h.PhongTroId == p.Id)
                    .OrderByDescending(h => h.LaAnhDaiDien)
                    .ThenBy(h => h.ThuTuHienThi)
                    .Select(h => h.DuongDanAnh)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }
        // tạo đường dẫn ảnh đưa vè thư mục gốc
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
    // lấy dữ liệu trả về giao diện cuối cùng của đăng bài
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
    // Xác nhận gửi duyệt, chuyển trạng thái về chờ duyệt, và lưu
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
    // Chưa gửi duyệt, lưu bài đươi trang thái nháp
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
        // Nếu có id tức là người dùng đã lưu và đang cập nhập nến phải thực hiện truy vấn
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
        // nếu chưa lưu đi thẳng đến tạo mới
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
                AnhDaiDien = _context.HinhAnhs
                    .Where(h => h.PhongTroId == p.Id)
                    .OrderByDescending(h => h.LaAnhDaiDien)
                    .ThenBy(h => h.ThuTuHienThi)
                    .Select(h => h.DuongDanAnh)
                    .FirstOrDefault()
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
        // Kiểm tra có dấu / ở đầu không hay là đường dẫn online
        return path.StartsWith('/') || path.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? path
            : "/" + path.TrimStart('/');
    }

    private static bool TryParseNguoiDungId(string userId, out int nguoiDungId) =>
        int.TryParse(userId, out nguoiDungId) && nguoiDungId > 0;
}
