using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.YeuCauThue;

namespace QLNhaTro.Controllers;

[Authorize(Roles = "NGUOI_THUE")]
public class YeuCauThueController : Controller
{
    private readonly PhongTroDaNangContext _context;

    public YeuCauThueController(PhongTroDaNangContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GuiYeuCau(int phongTroId)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdText, out var nguoiDungId)) return RedirectToAction("Login", "Account");

        var phong = await NapThongTinPhongChoYeuCauAsync(phongTroId);
        if (phong is null) return NotFound();

        var nguoiThue = await _context.NguoiThues.AsNoTracking().Include(n => n.NguoiDung).FirstOrDefaultAsync(n => n.NguoiDungId == nguoiDungId);
        if (nguoiThue is not null)
        {
            phong.HoTenLienHe = nguoiThue.NguoiDung.HoTen;
            phong.SoDienThoaiLienHe = nguoiThue.NguoiDung.SoDienThoai ?? string.Empty;
        }

        return View(phong);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GuiYeuCau(GuiYeuCauThueViewModel model)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdText, out var nguoiDungId)) return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            await NapThongTinPhongChoYeuCauAsync(model);
            return View(model);
        }

        var nguoiThueId = await _context.NguoiThues
            .Where(n => n.NguoiDungId == nguoiDungId)
            .Select(n => n.Id)
            .FirstOrDefaultAsync();
        if (nguoiThueId <= 0) return Forbid();

        var phongTonTai = await _context.PhongTros.AnyAsync(p => p.Id == model.PhongTroId);
        if (!phongTonTai) return NotFound();

        var datThue = new QLNhaTro.Domain.DatThue
        {
            NguoiThueId = nguoiThueId,
            PhongTroId = model.PhongTroId,
            HoTenLienHe = model.HoTenLienHe.Trim(),
            SoDienThoaiLienHe = model.SoDienThoaiLienHe.Trim(),
            NgayMuonXemPhong = model.NgayMuonXemPhong,
            LoiNhan = model.LoiNhan?.Trim(),
            TrangThai = YeuCauThueStatus.Moi,
            NgayTao = DateTime.Now
        };

        _context.DatThues.Add(datThue);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Không thể gửi yêu cầu thuê. Vui lòng kiểm tra trạng thái yêu cầu hoặc liên hệ quản trị viên.");
            await NapThongTinPhongChoYeuCauAsync(model);
            return View(model);
        }

        TempData["Success"] = "Đã gửi yêu cầu thuê. Vui lòng chờ chủ trọ phản hồi.";
        return RedirectToAction(nameof(DanhSach));
    }

    [HttpGet]
    public async Task<IActionResult> DanhSach(string? tuKhoa, string? trangThai)
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdText, out var nguoiDungId)) return RedirectToAction("Login", "Account");

        var nguoiThueId = await _context.NguoiThues.Where(n => n.NguoiDungId == nguoiDungId).Select(n => n.Id).FirstOrDefaultAsync();
        var query = _context.DatThues.AsNoTracking().Where(d => d.NguoiThueId == nguoiThueId);

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var kw = tuKhoa.Trim();
            query = query.Where(d => d.HoTenLienHe.Contains(kw) || d.PhongTro.TenPhong.Contains(kw) || d.PhongTro.NhaTro.TenNhaTro.Contains(kw) || d.PhongTro.NhaTro.DiaChiChiTiet.Contains(kw));
        }
        if (!string.IsNullOrWhiteSpace(trangThai)) query = query.Where(d => d.TrangThai == trangThai.Trim());

        var rows = await query
            .OrderByDescending(d => d.NgayTao)
            .Select(d => new
            {
                d.Id,
                TieuDePhong = d.PhongTro.TenPhong,
                TenNhaTro = d.PhongTro.NhaTro.TenNhaTro,
                DiaChi = d.PhongTro.NhaTro.DiaChiChiTiet,
                d.PhongTro.GiaThueThang,
                d.NgayTao,
                d.NgayMuonXemPhong,
                d.TrangThai,
                d.PhongTroId
            })
            .ToListAsync();

        var phongTroIds = rows.Select(x => x.PhongTroId).ToList();
        var anhMap = await _context.HinhAnhs.AsNoTracking()
            .Where(h => phongTroIds.Contains(h.PhongTroId))
            .OrderByDescending(h => h.LaAnhDaiDien).ThenBy(h => h.ThuTuHienThi)
            .Select(h => new { h.PhongTroId, h.DuongDanAnh })
            .ToListAsync();
        var dict = anhMap.GroupBy(x => x.PhongTroId).ToDictionary(g => g.Key, g => g.First().DuongDanAnh);

        var model = new YeuCauThueListPageViewModel
        {
            TuKhoa = tuKhoa,
            TrangThai = trangThai,
            DanhSachTrangThai = new List<SelectListItem>
            {
                new SelectListItem("Tất cả trạng thái", ""),
                new SelectListItem(YeuCauThueStatus.GetDisplayName(YeuCauThueStatus.Moi), YeuCauThueStatus.Moi),
                new SelectListItem(YeuCauThueStatus.GetDisplayName(YeuCauThueStatus.DongY), YeuCauThueStatus.DongY),
                new SelectListItem(YeuCauThueStatus.GetDisplayName(YeuCauThueStatus.TuChoi), YeuCauThueStatus.TuChoi)
            },
            YeuCaus = rows.Select(x =>
            {
                dict.TryGetValue(x.PhongTroId, out var anh);
                return new YeuCauThueListItemViewModel
                {
                    Id = x.Id,
                    TieuDePhong = x.TieuDePhong,
                    TenNhaTro = x.TenNhaTro,
                    DiaChi = x.DiaChi,
                    GiaThue = x.GiaThueThang,
                    NgayGui = x.NgayTao,
                    NgayMuonXemPhong = x.NgayMuonXemPhong,
                    TrangThai = x.TrangThai,
                    TrangThaiText = YeuCauThueStatus.GetDisplayName(x.TrangThai),
                    AnhDaiDien = anh
                };
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult ChiTiet() => RedirectToAction(nameof(DanhSach));

    [HttpGet]
    public IActionResult HuyYeuCau() => RedirectToAction(nameof(DanhSach));

    private async Task<GuiYeuCauThueViewModel?> NapThongTinPhongChoYeuCauAsync(int phongTroId)
    {
        return await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.Id == phongTroId)
            .Select(p => new GuiYeuCauThueViewModel
            {
                PhongTroId = p.Id,
                TieuDePhong = p.TenPhong,
                TenNhaTro = p.NhaTro.TenNhaTro,
                DiaChi = p.NhaTro.DiaChiChiTiet,
                GiaThue = p.GiaThueThang,
                DienTich = p.DienTich,
                AnhDaiDien = _context.HinhAnhs.Where(h => h.PhongTroId == p.Id)
                    .OrderByDescending(h => h.LaAnhDaiDien)
                    .ThenBy(h => h.ThuTuHienThi)
                    .Select(h => h.DuongDanAnh)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();
    }

    private async Task NapThongTinPhongChoYeuCauAsync(GuiYeuCauThueViewModel model)
    {
        var reloaded = await NapThongTinPhongChoYeuCauAsync(model.PhongTroId);
        if (reloaded is null) return;

        model.TieuDePhong = reloaded.TieuDePhong;
        model.TenNhaTro = reloaded.TenNhaTro;
        model.DiaChi = reloaded.DiaChi;
        model.GiaThue = reloaded.GiaThue;
        model.DienTich = reloaded.DienTich;
        model.AnhDaiDien = reloaded.AnhDaiDien;
    }
}
