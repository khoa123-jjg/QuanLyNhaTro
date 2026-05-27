using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaiDangEntity = QLNhaTro.Domain.BaiDang;
using NhaTroEntity = QLNhaTro.Domain.NhaTro;
using QLNhaTro.Data;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.Admin.BaiDuyet;

namespace QLNhaTro.Repositories.Admin;

public class AdminBaiDuyetRepository : IAdminBaiDuyetRepository
{
    private readonly PhongTroDaNangContext _context;

    public AdminBaiDuyetRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<AdminBaiChoDuyetListPageViewModel> GetDanhSachChoDuyetAsync(string? tuKhoa, int? nhaTroId)
    {
        var page = new AdminBaiChoDuyetListPageViewModel
        {
            TuKhoa = tuKhoa,
            NhaTroId = nhaTroId,
            DanhSachNhaTro = await GetDanhSachNhaTroAsync()
        };

        var query = _context.BaiDangs
            .AsNoTracking()
            .Where(b => b.TrangThaiDuyet == BaiDangStatus.ChoDuyet);

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(b =>
                b.TieuDe.Contains(keyword) ||
                b.PhongTro.MaPhong.Contains(keyword) ||
                b.PhongTro.NhaTro.TenNhaTro.Contains(keyword) ||
                b.PhongTro.NhaTro.ChuNhaTro.NguoiDung.HoTen.Contains(keyword));
        }

        if (nhaTroId is > 0)
        {
            query = query.Where(b => b.PhongTro.NhaTroId == nhaTroId.Value);
        }

        var items = await query
            .OrderByDescending(b => b.NgayTao)
            .Select(b => new AdminBaiChoDuyetListItemViewModel
            {
                Id = b.Id,
                TieuDe = b.TieuDe,
                TenChuTro = b.PhongTro.NhaTro.ChuNhaTro.NguoiDung.HoTen,
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                NgayGuiDuyet = b.NgayTao,
                TrangThaiDuyet = b.TrangThaiDuyet,
                AnhDaiDien = b.PhongTro.HinhAnh != null ? b.PhongTro.HinhAnh.DuongDanAnh : null
            })
            .ToListAsync();

        for (var i = 0; i < items.Count; i++)
        {
            items[i].Stt = i + 1;
        }

        page.BaiDangChoDuyet = items;
        return page;
    }

    public async Task<AdminChiTietBaiChoDuyetViewModel?> GetChiTietChoDuyetAsync(int id)
    {
        var row = await _context.BaiDangs
            .AsNoTracking()
            .Where(b => b.Id == id && b.TrangThaiDuyet == BaiDangStatus.ChoDuyet)
            .Select(b => new
            {
                b.Id,
                b.TieuDe,
                b.NoiDung,
                TenChuTro = b.PhongTro.NhaTro.ChuNhaTro.NguoiDung.HoTen,
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                b.PhongTro.MaPhong,
                b.PhongTro.TenPhong,
                DiaChi = b.PhongTro.NhaTro.DiaChiChiTiet,
                b.PhongTro.DienTich,
                b.PhongTro.GiaThueThang,
                b.PhongTro.TienCoc,
                b.PhongTro.SoNguoiToiDa,
                NgayGuiDuyet = b.NgayTao,
                b.TrangThaiDuyet,
                DuongDanAnh = b.PhongTro.HinhAnh != null ? b.PhongTro.HinhAnh.DuongDanAnh : null,
                DanhSachTienNghi = b.PhongTro.TienNghis
                    .OrderBy(x => x.TenTienNghi)
                    .Select(x => x.TenTienNghi)
                    .ToList()
            })
            .Select(b => new AdminChiTietBaiChoDuyetViewModel
            {
                Id = b.Id,
                TieuDe = b.TieuDe,
                NoiDung = b.NoiDung,
                TenChuTro = b.TenChuTro,
                TenNhaTro = b.TenNhaTro,
                MaPhong = b.MaPhong,
                TenPhong = b.TenPhong,
                DiaChi = b.DiaChi,
                DienTich = b.DienTich,
                GiaThueThang = b.GiaThueThang,
                TienCoc = b.TienCoc,
                SoNguoiToiDa = b.SoNguoiToiDa,
                NgayGuiDuyet = b.NgayGuiDuyet,
                TrangThaiDuyet = b.TrangThaiDuyet,
                DanhSachAnh = b.DuongDanAnh == null ? new List<string>() : new List<string> { b.DuongDanAnh! },
                DanhSachTienNghi = b.DanhSachTienNghi
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        return row;
    }

    public async Task<AdminTuChoiBaiViewModel?> GetTuChoiViewModelAsync(int id)
    {
        var row = await _context.BaiDangs
            .AsNoTracking()
            .Where(b => b.Id == id && b.TrangThaiDuyet == BaiDangStatus.ChoDuyet)
            .Select(b => new AdminTuChoiBaiViewModel
            {
                Id = b.Id,
                TieuDe = b.TieuDe,
                TenChuTro = b.PhongTro.NhaTro.ChuNhaTro.NguoiDung.HoTen,
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                NgayGuiDuyet = b.NgayTao
            })
            .FirstOrDefaultAsync();

        return row;
    }

    public async Task<(bool Success, string Message)> DuyetBaiAsync(int id, string adminUserId)
    {
        if (!int.TryParse(adminUserId, out var adminId))
        {
            return (false, "Không xác định được tài khoản admin.");
        }

        var baiDang = await _context.BaiDangs.FirstOrDefaultAsync(x => x.Id == id);
        if (baiDang is null)
        {
            return (false, "Không tìm thấy bài đăng.");
        }

        if (!string.Equals(baiDang.TrangThaiDuyet, BaiDangStatus.ChoDuyet, StringComparison.OrdinalIgnoreCase))
        {
            return (false, "Chỉ có thể duyệt bài đang ở trạng thái chờ duyệt.");
        }

        baiDang.TrangThaiDuyet = BaiDangStatus.DaDuyet;
        baiDang.NguoiDuyetId = adminId;
        baiDang.NgayDuyet = DateTime.Now;
        baiDang.LyDoTuChoi = null;
        baiDang.NgayCapNhat = DateTime.Now;

        await _context.SaveChangesAsync();
        return (true, "Đã duyệt bài đăng.");
    }

    public async Task<(bool Success, string Message)> TuChoiBaiAsync(int id, string adminUserId, string lyDoTuChoi)
    {
        if (!int.TryParse(adminUserId, out var adminId))
        {
            return (false, "Không xác định được tài khoản admin.");
        }

        var baiDang = await _context.BaiDangs.FirstOrDefaultAsync(x => x.Id == id);
        if (baiDang is null)
        {
            return (false, "Không tìm thấy bài đăng.");
        }

        if (!string.Equals(baiDang.TrangThaiDuyet, BaiDangStatus.ChoDuyet, StringComparison.OrdinalIgnoreCase))
        {
            return (false, "Chỉ có thể từ chối bài đang ở trạng thái chờ duyệt.");
        }

        baiDang.TrangThaiDuyet = BaiDangStatus.BiTuChoi;
        baiDang.LyDoTuChoi = lyDoTuChoi.Trim();
        baiDang.NguoiDuyetId = adminId;
        baiDang.NgayDuyet = DateTime.Now;
        baiDang.NgayCapNhat = DateTime.Now;

        await _context.SaveChangesAsync();
        return (true, "Đã từ chối bài đăng.");
    }

    private async Task<List<SelectListItem>> GetDanhSachNhaTroAsync()
    {
        return await _context.NhaTros
            .AsNoTracking()
            .OrderBy(x => x.TenNhaTro)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.TenNhaTro
            })
            .ToListAsync();
    }

}
