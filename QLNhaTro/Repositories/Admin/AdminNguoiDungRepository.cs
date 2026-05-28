using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.Admin.NguoiDung;

namespace QLNhaTro.Repositories.Admin;

public class AdminNguoiDungRepository : IAdminNguoiDungRepository
{
    private readonly PhongTroDaNangContext _context;

    public AdminNguoiDungRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<AdminNguoiDungListPageViewModel> GetDanhSachNguoiDungAsync(string? tuKhoa, string? vaiTro, string? trangThai)
    {
        var page = new AdminNguoiDungListPageViewModel
        {
            TuKhoa = tuKhoa,
            VaiTro = vaiTro,
            TrangThai = trangThai,
            DanhSachVaiTro = await GetDanhSachVaiTroAsync(),
            DanhSachTrangThai = GetDanhSachTrangThai()
        };

        var query = _context.NguoiDungs
            .AsNoTracking()
            .Include(u => u.NguoiDungVaiTros)
                .ThenInclude(nv => nv.VaiTro)
            .AsSplitQuery();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(u =>
                u.HoTen.Contains(keyword) ||
                u.Email.Contains(keyword) ||
                (u.SoDienThoai != null && u.SoDienThoai.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(vaiTro))
        {
            var roleKey = vaiTro.Trim().ToUpperInvariant();
            query = query.Where(u => u.NguoiDungVaiTros.Any(nv => nv.VaiTro.TenVaiTro == roleKey));
        }

        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            query = query.Where(u => u.TrangThai == trangThai.Trim());
        }

        var rows = await query
            .OrderByDescending(u => u.NgayTao)
            .Select(u => new
            {
                u.Id,
                u.HoTen,
                u.Email,
                u.SoDienThoai,
                u.TrangThai,
                u.NgayTao,
                VaiTros = u.NguoiDungVaiTros
                    .OrderBy(nv => nv.NgayGan)
                    .Select(nv => nv.VaiTro.TenVaiTro)
                    .ToList()
            })
            .ToListAsync();

        page.NguoiDungs = rows
            .Select((u, index) => new AdminNguoiDungListItemViewModel
            {
                Id = u.Id,
                Stt = index + 1,
                HoTen = u.HoTen,
                Email = u.Email,
                SoDienThoai = u.SoDienThoai,
                VaiTro = string.Join(", ", u.VaiTros),
                VaiTroText = string.Join(", ", u.VaiTros.Select(GetVaiTroText)),
                TrangThai = u.TrangThai,
                TrangThaiText = NguoiDungStatus.GetDisplayName(u.TrangThai),
                NgayTao = u.NgayTao
            })
            .ToList();

        return page;
    }

    public async Task<AdminNguoiDungDetailViewModel?> GetChiTietNguoiDungAsync(int id)
    {
        var row = await _context.NguoiDungs
            .AsNoTracking()
            .Include(u => u.NguoiDungVaiTros)
                .ThenInclude(nv => nv.VaiTro)
            .Include(u => u.NguoiThue)
            .Include(u => u.ChuNhaTro)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (row is null)
        {
            return null;
        }

        var vaiTros = row.NguoiDungVaiTros
            .OrderBy(nv => nv.NgayGan)
            .Select(nv => nv.VaiTro.TenVaiTro)
            .ToList();

        var model = new AdminNguoiDungDetailViewModel
        {
            Id = row.Id,
            HoTen = row.HoTen,
            Email = row.Email,
            SoDienThoai = row.SoDienThoai,
            VaiTro = string.Join(", ", vaiTros),
            VaiTroText = string.Join(", ", vaiTros.Select(GetVaiTroText)),
            TrangThai = row.TrangThai,
            TrangThaiText = NguoiDungStatus.GetDisplayName(row.TrangThai),
            NgayTao = row.NgayTao,
            GhiChu = row.GhiChu
        };

        if (row.NguoiThue is not null)
        {
            model.NgheNghiep = row.NguoiThue.NgheNghiep;
            model.NhuCauThue = row.NguoiThue.NhuCauThue;
        }

        if (row.ChuNhaTro is not null)
        {
            model.TrangThaiHoSo = row.ChuNhaTro.TrangThaiHoSo;
        }

        return model;
    }

    public async Task<AdminNguoiDungEditViewModel?> GetChinhSuaNguoiDungAsync(int id)
    {
        var row = await _context.NguoiDungs
            .AsNoTracking()
            .Include(u => u.NguoiDungVaiTros)
                .ThenInclude(nv => nv.VaiTro)
            .Include(u => u.NguoiThue)
            .Include(u => u.ChuNhaTro)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (row is null)
        {
            return null;
        }

        var vaiTros = row.NguoiDungVaiTros
            .OrderBy(nv => nv.NgayGan)
            .Select(nv => nv.VaiTro.TenVaiTro)
            .ToList();

        var vaiTroPrimary = vaiTros.FirstOrDefault() ?? string.Empty;

        var model = new AdminNguoiDungEditViewModel
        {
            Id = row.Id,
            HoTen = row.HoTen,
            Email = row.Email,
            SoDienThoai = row.SoDienThoai,
            TrangThai = row.TrangThai,
            GhiChu = row.GhiChu,
            VaiTro = vaiTroPrimary,
            VaiTroText = string.Join(", ", vaiTros.Select(GetVaiTroText)),
            DanhSachTrangThai = GetDanhSachTrangThai().Where(x => !string.IsNullOrEmpty(x.Value)).ToList(),
            DanhSachTrangThaiHoSo = GetDanhSachTrangThaiHoSo()
        };

        if (row.NguoiThue is not null)
        {
            model.NgheNghiep = row.NguoiThue.NgheNghiep;
            model.NhuCauThue = row.NguoiThue.NhuCauThue;
        }

        if (row.ChuNhaTro is not null)
        {
            model.TrangThaiHoSo = row.ChuNhaTro.TrangThaiHoSo;
        }

        return model;
    }

    public async Task<(bool Success, string Message)> ChinhSuaNguoiDungAsync(AdminNguoiDungEditViewModel model)
    {
        var row = await _context.NguoiDungs
            .Include(u => u.NguoiThue)
            .Include(u => u.ChuNhaTro)
            .FirstOrDefaultAsync(u => u.Id == model.Id);

        if (row is null)
        {
            return (false, "Không tìm thấy người dùng.");
        }

        var email = model.Email.Trim();
        var soDienThoai = model.SoDienThoai?.Trim();

        var emailDaTonTai = await _context.NguoiDungs.AnyAsync(u => u.Email == email && u.Id != model.Id);
        if (emailDaTonTai)
        {
            return (false, "Email đã được sử dụng bởi người dùng khác.");
        }

        if (!string.IsNullOrWhiteSpace(soDienThoai))
        {
            var sdtDaTonTai = await _context.NguoiDungs.AnyAsync(u => u.SoDienThoai == soDienThoai && u.Id != model.Id);
            if (sdtDaTonTai)
            {
                return (false, "Số điện thoại đã được sử dụng bởi người dùng khác.");
            }
        }

        row.HoTen = model.HoTen.Trim();
        row.Email = email;
        row.SoDienThoai = soDienThoai;
        row.TrangThai = model.TrangThai;
        row.GhiChu = model.GhiChu?.Trim();
        row.NgayCapNhat = DateTime.Now;

        if (row.NguoiThue is not null)
        {
            row.NguoiThue.NgheNghiep = model.NgheNghiep?.Trim();
            row.NguoiThue.NhuCauThue = model.NhuCauThue?.Trim();
            row.NguoiThue.NgayCapNhat = DateTime.Now;
        }

        if (row.ChuNhaTro is not null)
        {
            row.ChuNhaTro.TrangThaiHoSo = model.TrangThaiHoSo ?? "HOAT_DONG";
            row.ChuNhaTro.NgayCapNhat = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return (true, "Cập nhật thông tin người dùng thành công.");
    }

    private static List<SelectListItem> GetDanhSachTrangThaiHoSo() =>
    [
        new SelectListItem { Value = "HOAT_DONG", Text = "Hoạt động" },
        new SelectListItem { Value = "BI_KHOA", Text = "Bị khóa" }
    ];

    public async Task<(bool Success, string Message)> KhoaMoKhoaNguoiDungAsync(int id, int adminId)
    {
        if (id == adminId)
        {
            return (false, "Không thể khóa hoặc mở khóa chính tài khoản của bạn.");
        }

        var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Id == id);
        if (nguoiDung is null)
        {
            return (false, "Không tìm thấy người dùng.");
        }

        nguoiDung.TrangThai = NguoiDungStatus.IsActive(nguoiDung.TrangThai)
            ? NguoiDungStatus.BiKhoa
            : NguoiDungStatus.HoatDong;

        if (_context.Entry(nguoiDung).Property(nameof(nguoiDung.NgayCapNhat)).Metadata is not null)
        {
            nguoiDung.NgayCapNhat = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return (true, NguoiDungStatus.IsActive(nguoiDung.TrangThai)
            ? "Đã mở khóa người dùng."
            : "Đã khóa người dùng.");
    }

    public async Task<(bool Success, string Message)> XoaNguoiDungAsync(int id, int adminId)
    {
        if (id == adminId)
        {
            return (false, "Không thể xóa chính tài khoản của bạn.");
        }

        var nguoiDung = await _context.NguoiDungs
            .Include(u => u.NguoiDungVaiTros)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (nguoiDung is null)
        {
            return (false, "Không tìm thấy người dùng.");
        }

        try
        {
            _context.NguoiDungs.Remove(nguoiDung);
            await _context.SaveChangesAsync();
            return (true, "Đã xóa người dùng.");
        }
        catch (DbUpdateException)
        {
            return (false, "Không thể xóa người dùng vì đã phát sinh dữ liệu liên quan. Bạn có thể khóa tài khoản này.");
        }
    }

    private async Task<List<SelectListItem>> GetDanhSachVaiTroAsync()
    {
        return await _context.VaiTros
            .AsNoTracking()
            .OrderBy(v => v.TenVaiTro)
            .Select(v => new SelectListItem
            {
                Value = v.TenVaiTro,
                Text = GetVaiTroText(v.TenVaiTro)
            })
            .ToListAsync();
    }

    private static List<SelectListItem> GetDanhSachTrangThai() =>
    [
        new SelectListItem { Value = "", Text = "Tất cả trạng thái" },
        new SelectListItem { Value = NguoiDungStatus.HoatDong, Text = NguoiDungStatus.GetDisplayName(NguoiDungStatus.HoatDong) },
        new SelectListItem { Value = NguoiDungStatus.BiKhoa, Text = NguoiDungStatus.GetDisplayName(NguoiDungStatus.BiKhoa) }
    ];

    private static string GetVaiTroText(string? vaiTro) => vaiTro?.Trim().ToUpperInvariant() switch
    {
        "ADMIN" => "Admin",
        "CHU_TRO" => "Chủ trọ",
        "NGUOI_THUE" => "Người thuê",
        _ => "Không xác định"
    };
}
