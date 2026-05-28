using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.Admin.Dashboard;

namespace QLNhaTro.Repositories.Admin;

public class AdminDashboardRepository : IAdminDashboardRepository
{
    private readonly PhongTroDaNangContext _context;

    public AdminDashboardRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardViewModel> GetDashboardAsync()
    {
        var sevenDaysAgo = DateTime.Now.AddDays(-7);

        var model = new AdminDashboardViewModel
        {
            TongNguoiDung = await _context.NguoiDungs.AsNoTracking().CountAsync(),
            NguoiDungMoiTrong7Ngay = await CountByNgayTaoAsync(_context.NguoiDungs, sevenDaysAgo),
            BaiChoDuyet = await _context.BaiDangs.AsNoTracking().CountAsync(b => b.TrangThaiDuyet == BaiDangStatus.ChoDuyet),
            BaiChoDuyetMoiTrong7Ngay = await _context.BaiDangs.AsNoTracking().CountAsync(b =>
                b.TrangThaiDuyet == BaiDangStatus.ChoDuyet &&
                b.NgayTao >= sevenDaysAgo),
            TongNhaTro = await _context.NhaTros.AsNoTracking().CountAsync(),
            NhaTroMoiTrong7Ngay = await CountByNgayTaoAsync(_context.NhaTros, sevenDaysAgo),
            TongTienNghi = await _context.TienNghis.AsNoTracking().CountAsync(),
            TienNghiMoiTrong7Ngay = await CountByNgayTaoAsync(_context.TienNghis, sevenDaysAgo)
        };

        var rows = await _context.BaiDangs
            .AsNoTracking()
            .Where(b => b.TrangThaiDuyet == BaiDangStatus.ChoDuyet)
            .OrderByDescending(b => b.NgayTao)
            .Select(b => new
            {
                b.Id,
                b.TieuDe,
                TenNguoiDang = b.PhongTro.NhaTro.ChuNhaTro.NguoiDung.HoTen,
                TenNhaTro = b.PhongTro.NhaTro.TenNhaTro,
                b.PhongTro.MaPhong,
                NgayGuiDuyet = b.NgayTao,
                b.PhongTroId,
                AnhDaiDien = _context.HinhAnhs
                    .Where(h => h.PhongTroId == b.PhongTroId)
                    .OrderByDescending(h => h.LaAnhDaiDien)
                    .ThenBy(h => h.ThuTuHienThi)
                    .Select(h => h.DuongDanAnh)
                    .FirstOrDefault()
            })
            .Take(5)
            .ToListAsync();

        model.BaiChoDuyetGanDay = rows.Select((row, index) => new AdminDashboardBaiChoDuyetItemViewModel
        {
            Id = row.Id,
            Stt = index + 1,
            TieuDe = row.TieuDe,
            TenNguoiDang = row.TenNguoiDang,
            TenNhaTro = row.TenNhaTro,
            MaPhong = row.MaPhong,
            NgayGuiDuyet = row.NgayGuiDuyet,
            AnhDaiDien = row.AnhDaiDien
        }).ToList();

        return model;
    }

    private static async Task<int> CountByNgayTaoAsync<TEntity>(IQueryable<TEntity> source, DateTime sevenDaysAgo)
        where TEntity : class
    {
        try
        {
            return await source.CountAsync(e => EF.Property<DateTime>(e, "NgayTao") >= sevenDaysAgo);
        }
        catch (InvalidOperationException)
        {
            return 0;
        }
        catch (ArgumentException)
        {
            return 0;
        }
    }
}
