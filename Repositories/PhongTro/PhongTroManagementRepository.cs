using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Domain;
using QLNhaTro.Models.PhongTro;

namespace QLNhaTro.Repositories.PhongTro;

public class PhongTroManagementRepository : IPhongTroManagementRepository
{
    private static readonly HashSet<string> TrangThaiHopLe =
    [
        "TRONG",
        "DANG_THUE",
        "DANG_SUA",
        "TAM_AN"
    ];

    private readonly PhongTroDaNangContext _context;

    public PhongTroManagementRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<List<SelectListItem>> GetDanhSachNhaTroCuaChuTroAsync(string userId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return [];
        }

        return await _context.NhaTros
            .AsNoTracking()
            .Where(n => n.ChuNhaTro.NguoiDungId == nguoiDungId)
            .OrderBy(n => n.TenNhaTro)
            .Select(n => new SelectListItem(n.TenNhaTro, n.Id.ToString()))
            .ToListAsync();
    }

    public async Task<PhongTroCreateUpdateViewModel?> GetPhongFormAsync(int? id, string userId)
    {
        var danhSachNhaTro = await GetDanhSachNhaTroCuaChuTroAsync(userId);

        if (id is null or <= 0)
        {
            return new PhongTroCreateUpdateViewModel
            {
                TrangThai = "TRONG",
                DanhSachNhaTro = danhSachNhaTro
            };
        }

        if (danhSachNhaTro.Count == 0 || !TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return null;
        }

        var row = await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.Id == id.Value)
            .Where(p => p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Select(p => new PhongTroCreateUpdateViewModel
            {
                Id = p.Id,
                NhaTroId = p.NhaTroId,
                MaPhong = p.MaPhong,
                TenPhong = p.TenPhong,
                Tang = p.Tang,
                DienTich = p.DienTich,
                GiaThueThang = p.GiaThueThang,
                TienCoc = p.TienCoc,
                SoNguoiToiDa = p.SoNguoiToiDa,
                MoTa = p.MoTa,
                TrangThai = p.TrangThai,
                GhiChu = p.GhiChu
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        row.DanhSachNhaTro = danhSachNhaTro;
        return row;
    }

    private static PhongTroManagementResult ThanhCong(string message) =>
        new() { Success = true, Message = message };

    private static PhongTroManagementResult ThatBai(string message) =>
        new() { Success = false, Message = message };

    private async Task<int?> LayChuNhaTroIdTheoNguoiDung(string userId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return null;
        }

        return await _context.ChuNhaTros
            .AsNoTracking()
            .Where(c => c.NguoiDungId == nguoiDungId)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<PhongTroManagementResult> CreatePhong(string userId, PhongTroCreateUpdateViewModel model)
    {
        var chuNhaTroId = await LayChuNhaTroIdTheoNguoiDung(userId);
        if (chuNhaTroId is null)
        {
            return ThatBai("Không xác định được hồ sơ chủ trọ.");
        }

        if (!TrangThaiHopLe.Contains(model.TrangThai))
        {
            return ThatBai("Trạng thái phòng không hợp lệ.");
        }

        if (!await NhaTroThuocChu(chuNhaTroId.Value, model.NhaTroId))
        {
            return ThatBai("Nhà trọ không thuộc quyền quản lý của bạn.");
        }

        var maPhong = model.MaPhong.Trim();
        if (await MaPhongDaTonTai(model.NhaTroId, maPhong, excludePhongId: null))
        {
            return ThatBai("Mã phòng đã tồn tại trong nhà trọ này.");
        }

        var now = DateTime.Now;
        var phong = new Domain.PhongTro
        {
            NhaTroId = model.NhaTroId,
            MaPhong = maPhong,
            TenPhong = model.TenPhong.Trim(),
            Tang = model.Tang,
            DienTich = model.DienTich,
            GiaThueThang = model.GiaThueThang,
            TienCoc = model.TienCoc,
            SoNguoiToiDa = model.SoNguoiToiDa,
            MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? null : model.MoTa.Trim(),
            TrangThai = model.TrangThai,
            GhiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim(),
            NgayTao = now,
            NgayCapNhat = now
        };

        _context.PhongTros.Add(phong);
        await _context.SaveChangesAsync();

        return ThanhCong("Thêm phòng thành công.");
    }

    public async Task<PhongTroManagementResult> UpdatePhongAsync(
        string userId,
        PhongTroCreateUpdateViewModel model)
    {
        if (model.Id <= 0)
        {
            return ThatBai("Mã phòng không hợp lệ.");
        }

        var chuNhaTroId = await LayChuNhaTroIdTheoNguoiDung(userId);
        if (chuNhaTroId is null)
        {
            return ThatBai("Không xác định được hồ sơ chủ trọ.");
        }

        if (!TrangThaiHopLe.Contains(model.TrangThai))
        {
            return ThatBai("Trạng thái phòng không hợp lệ.");
        }

        if (!await NhaTroThuocChu(chuNhaTroId.Value, model.NhaTroId))
        {
            return ThatBai("Nhà trọ không thuộc quyền quản lý của bạn.");
        }

        var phong = await _context.PhongTros
            .Include(p => p.NhaTro)
            .FirstOrDefaultAsync(p =>
                p.Id == model.Id
                && p.NhaTro.ChuNhaTroId == chuNhaTroId.Value);

        if (phong is null)
        {
            return ThatBai("Không tìm thấy phòng hoặc bạn không có quyền sửa.");
        }

        var maPhong = model.MaPhong.Trim();
        if (await MaPhongDaTonTai(model.NhaTroId, maPhong, excludePhongId: model.Id))
        {
            return ThatBai("Mã phòng đã tồn tại trong nhà trọ này.");
        }

        phong.NhaTroId = model.NhaTroId;
        phong.MaPhong = maPhong;
        phong.TenPhong = model.TenPhong.Trim();
        phong.Tang = model.Tang;
        phong.DienTich = model.DienTich;
        phong.GiaThueThang = model.GiaThueThang;
        phong.TienCoc = model.TienCoc;
        phong.SoNguoiToiDa = model.SoNguoiToiDa;
        phong.MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? null : model.MoTa.Trim();
        phong.TrangThai = model.TrangThai;
        phong.GhiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim();
        phong.NgayCapNhat = DateTime.Now;

        await _context.SaveChangesAsync();

        return ThanhCong("Cập nhật phòng thành công.");
    }



    private async Task<bool> NhaTroThuocChu(int chuNhaTroId, int nhaTroId) =>
        await _context.NhaTros
            .AsNoTracking()
            .AnyAsync(n => n.Id == nhaTroId && n.ChuNhaTroId == chuNhaTroId);

    private async Task<bool> MaPhongDaTonTai(int nhaTroId, string maPhong, int? excludePhongId)
    {
        var query = _context.PhongTros
            .AsNoTracking()
            .Where(p => p.NhaTroId == nhaTroId && p.MaPhong == maPhong);

        if (excludePhongId is > 0)
        {
            query = query.Where(p => p.Id != excludePhongId.Value);
        }

        return await query.AnyAsync();
    }

    private static bool TryParseNguoiDungId(string userId, out int nguoiDungId) =>
        int.TryParse(userId, out nguoiDungId) && nguoiDungId > 0;


}
