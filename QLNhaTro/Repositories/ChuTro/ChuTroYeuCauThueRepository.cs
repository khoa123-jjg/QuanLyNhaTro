using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.ChuTro.YeuCauThue;

namespace QLNhaTro.Repositories.ChuTro;

public class ChuTroYeuCauThueRepository : IChuTroYeuCauThueRepository
{
    private readonly PhongTroDaNangContext _context;

    public ChuTroYeuCauThueRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<ChuTroYeuCauThueListPageViewModel> GetDanhSachAsync(int nguoiDungId, string? trangThai, int? nhaTroId, string? sapXep)
    {
        var query = _context.DatThues
            .AsNoTracking()
            .Where(d => d.PhongTro.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId);

        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            query = query.Where(d => d.TrangThai == trangThai);
        }

        if (nhaTroId is > 0)
        {
            query = query.Where(d => d.PhongTro.NhaTroId == nhaTroId.Value);
        }

        query = string.Equals(sapXep, "cu-nhat", StringComparison.OrdinalIgnoreCase)
            ? query.OrderBy(d => d.NgayTao)
            : query.OrderByDescending(d => d.NgayTao);

        var rows = await query
            .Select(d => new
            {
                d.Id,
                d.HoTenLienHe,
                d.SoDienThoaiLienHe,
                Email = d.NguoiThue.NguoiDung.Email,
                d.PhongTroId,
                d.PhongTro.MaPhong,
                d.PhongTro.TenPhong,
                d.PhongTro.NhaTro.TenNhaTro,
                d.PhongTro.GiaThueThang,
                d.NgayTao,
                d.NgayMuonXemPhong,
                d.TrangThai,
                AnhDaiDien = _context.HinhAnhs
                    .Where(h => h.PhongTroId == d.PhongTroId)
                    .OrderByDescending(h => h.LaAnhDaiDien)
                    .ThenBy(h => h.ThuTuHienThi)
                    .Select(h => h.DuongDanAnh)
                    .FirstOrDefault(),
                CoTheXuLy = YeuCauThueStatus.CoTheXuLy(d.TrangThai)
            })
            .ToListAsync();

        var danhSachNhaTro = await _context.NhaTros
            .AsNoTracking()
            .Where(n => n.ChuNhaTro.NguoiDungId == nguoiDungId)
            .OrderBy(n => n.TenNhaTro)
            .Select(n => new SelectListItem(n.TenNhaTro, n.Id.ToString()))
            .ToListAsync();

        var model = new ChuTroYeuCauThueListPageViewModel
        {
            TrangThai = trangThai,
            NhaTroId = nhaTroId,
            SapXep = sapXep,
            DanhSachTrangThai = new List<SelectListItem>
            {
                new("Tất cả trạng thái", ""),
                new(YeuCauThueStatus.GetDisplayName(YeuCauThueStatus.Moi), YeuCauThueStatus.Moi),
                new(YeuCauThueStatus.GetDisplayName(YeuCauThueStatus.ChuTroDongY), YeuCauThueStatus.ChuTroDongY),
                new(YeuCauThueStatus.GetDisplayName(YeuCauThueStatus.ChuTroTuChoi), YeuCauThueStatus.ChuTroTuChoi)
            },
            DanhSachNhaTro = danhSachNhaTro,
            YeuCaus = rows.Select(x => new ChuTroYeuCauThueListItemViewModel
            {
                Id = x.Id,
                HoTenNguoiThue = string.IsNullOrWhiteSpace(x.HoTenLienHe) ? (x.Email ?? string.Empty) : x.HoTenLienHe,
                SoDienThoaiLienHe = x.SoDienThoaiLienHe,
                EmailNguoiThue = x.Email,
                PhongTroId = x.PhongTroId,
                MaPhong = x.MaPhong,
                TenPhong = x.TenPhong,
                TenNhaTro = x.TenNhaTro,
                GiaThueThang = x.GiaThueThang,
                AnhDaiDien = TaoDuongDanAnh(x.AnhDaiDien),
                NgayGui = x.NgayTao,
                NgayMuonXemPhong = x.NgayMuonXemPhong,
                TrangThai = x.TrangThai,
                TrangThaiText = YeuCauThueStatus.GetDisplayName(x.TrangThai),
                CoTheXuLy = x.CoTheXuLy
            }).ToList()
        };

        return model;
    }

    public async Task<ChuTroChiTietYeuCauThueViewModel?> GetChiTietAsync(int id, int nguoiDungId)
    {
        var row = await _context.DatThues
            .AsNoTracking()
            .Where(d => d.Id == id && d.PhongTro.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Select(d => new ChuTroChiTietYeuCauThueViewModel
            {
                Id = d.Id,
                HoTenNguoiThue = string.IsNullOrWhiteSpace(d.HoTenLienHe) ? d.NguoiThue.NguoiDung.HoTen : d.HoTenLienHe,
                SoDienThoaiLienHe = d.SoDienThoaiLienHe,
                EmailNguoiThue = d.NguoiThue.NguoiDung.Email,
                PhongTroId = d.PhongTroId,
                MaPhong = d.PhongTro.MaPhong,
                TenPhong = d.PhongTro.TenPhong,
                TenNhaTro = d.PhongTro.NhaTro.TenNhaTro,
                DiaChi = d.PhongTro.NhaTro.DiaChiChiTiet,
                GiaThueThang = d.PhongTro.GiaThueThang,
                DienTich = d.PhongTro.DienTich,
                SoNguoiToiDa = d.PhongTro.SoNguoiToiDa,
                AnhDaiDien = _context.HinhAnhs
                    .Where(h => h.PhongTroId == d.PhongTroId)
                    .OrderByDescending(h => h.LaAnhDaiDien)
                    .ThenBy(h => h.ThuTuHienThi)
                    .Select(h => h.DuongDanAnh)
                    .FirstOrDefault(),
                LoiNhan = d.LoiNhan,
                NgayGui = d.NgayTao,
                NgayMuonXemPhong = d.NgayMuonXemPhong,
                TrangThai = d.TrangThai,
                TrangThaiText = YeuCauThueStatus.GetDisplayName(d.TrangThai),
                CoTheXuLy = YeuCauThueStatus.CoTheXuLy(d.TrangThai),
                GhiChuChuTro = d.GhiChuChuTro,
                LyDoTuChoi = d.LyDoTuChoi
            })
            .FirstOrDefaultAsync();

        if (row is not null)
        {
            row.AnhDaiDien = TaoDuongDanAnh(row.AnhDaiDien);
        }

        return row;
    }

    public async Task<ChuTroXuLyYeuCauThueViewModel?> GetXuLyAsync(int id, int nguoiDungId)
    {
        return await _context.DatThues
            .AsNoTracking()
            .Where(d => d.Id == id && d.PhongTro.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Select(d => new ChuTroXuLyYeuCauThueViewModel
            {
                Id = d.Id,
                HoTenNguoiThue = string.IsNullOrWhiteSpace(d.HoTenLienHe) ? d.NguoiThue.NguoiDung.HoTen : d.HoTenLienHe,
                SoDienThoaiLienHe = d.SoDienThoaiLienHe,
                MaPhong = d.PhongTro.MaPhong,
                TenPhong = d.PhongTro.TenPhong,
                TenNhaTro = d.PhongTro.NhaTro.TenNhaTro,
                GiaThueThang = d.PhongTro.GiaThueThang,
                AnhDaiDien = _context.HinhAnhs.Where(h => h.PhongTroId == d.PhongTroId)
                    .OrderByDescending(h => h.LaAnhDaiDien)
                    .ThenBy(h => h.ThuTuHienThi)
                    .Select(h => h.DuongDanAnh)
                    .FirstOrDefault(),
                NgayGui = d.NgayTao,
                NgayMuonXemPhong = d.NgayMuonXemPhong,
                TrangThaiHienTai = d.TrangThai,
                TrangThaiHienTaiText = YeuCauThueStatus.GetDisplayName(d.TrangThai)
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message)> XuLyYeuCauAsync(int id, int nguoiDungId, string trangThaiXuLy, string? ghiChuChuTro, string? lyDoTuChoi)
    {
        if (trangThaiXuLy != YeuCauThueStatus.ChuTroDongY && trangThaiXuLy != YeuCauThueStatus.ChuTroTuChoi)
        {
            return (false, "Trạng thái xử lý không hợp lệ.");
        }

        if (trangThaiXuLy == YeuCauThueStatus.ChuTroTuChoi && string.IsNullOrWhiteSpace(lyDoTuChoi))
        {
            return (false, "Vui lòng nhập lý do từ chối.");
        }

        var datThue = await _context.DatThues
            .Include(d => d.PhongTro)
                .ThenInclude(p => p.NhaTro)
                    .ThenInclude(n => n.ChuNhaTro)
            .FirstOrDefaultAsync(d => d.Id == id && d.PhongTro.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId);

        if (datThue is null)
        {
            return (false, "Không tìm thấy yêu cầu thuê hoặc bạn không có quyền xử lý.");
        }

        if (!YeuCauThueStatus.CoTheXuLy(datThue.TrangThai))
        {
            return (false, "Yêu cầu thuê này đã được xử lý.");
        }

        datThue.TrangThai = trangThaiXuLy;
        if (datThue.GetType().GetProperty("NgayXuLy") is not null)
        {
            datThue.GetType().GetProperty("NgayXuLy")!.SetValue(datThue, DateTime.Now);
        }
        if (datThue.GetType().GetProperty("NgayCapNhat") is not null)
        {
            datThue.GetType().GetProperty("NgayCapNhat")!.SetValue(datThue, DateTime.Now);
        }
        if (!string.IsNullOrWhiteSpace(ghiChuChuTro))
        {
            datThue.GhiChuChuTro = ghiChuChuTro.Trim();
        }
        if (trangThaiXuLy == YeuCauThueStatus.ChuTroTuChoi && !string.IsNullOrWhiteSpace(lyDoTuChoi))
        {
            datThue.LyDoTuChoi = lyDoTuChoi.Trim();
        }

        await _context.SaveChangesAsync();

        return trangThaiXuLy == YeuCauThueStatus.ChuTroDongY
            ? (true, "Đã đồng ý yêu cầu thuê.")
            : (true, "Đã từ chối yêu cầu thuê.");
    }

    private static string? TaoDuongDanAnh(string? duongDanAnh)
    {
        if (string.IsNullOrWhiteSpace(duongDanAnh)) return null;
        var path = duongDanAnh.Trim();
        return path.StartsWith('/') || path.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? path : "/" + path.TrimStart('/');
    }
}
