using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Models.NhaTro;

namespace QLNhaTro.Repositories.NhaTro;

public class NhaTroRepository : INhaTroRepository
{
    private const string TrangThaiHienThi = "HIEN_THI";
    private const string TrangThaiMacDinh = "HOAT_DONG";

    private static readonly HashSet<string> TrangThaiNhaTroHopLe =
    [
        "HOAT_DONG",
        "TAM_AN",
        "NGUNG_HOAT_DONG"
    ];

    private readonly PhongTroDaNangContext _context;

    public NhaTroRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<List<NhaTroListItemViewModel>> GetNhaTroCuaChuTro(string userId)
    {
        var chuNhaTroId = await LayChuNhaTroIdTheoUser(userId);
        if (chuNhaTroId is null)
        {
            return [];
        }

        var rows = await _context.NhaTros
            .AsNoTracking()
            .Where(n => n.ChuNhaTroId == chuNhaTroId.Value)
            .OrderByDescending(n => n.NgayTao)
            .Select(n => new
            {
                n.Id,
                n.TenNhaTro,
                n.SoNha,
                n.DiaChiChiTiet,
                n.TrangThai,
                n.NgayTao,
                TenDuong = n.DuongPho != null ? n.DuongPho.TenDuong : null,
                TenXa = n.DuongPho != null ? n.DuongPho.Xa.TenXahuyen : null,
                TenQuanhuyen = n.DuongPho != null ? n.DuongPho.Xa.Quanhuyen.TenQuanhuyen : null,
                ThanhPho = n.DuongPho != null ? n.DuongPho.Xa.Quanhuyen.ThanhPho : null,
                SoPhong = n.PhongTros.Count
            })
            .ToListAsync();

        return rows.Select(r => new NhaTroListItemViewModel
        {
            Id = r.Id,
            TenNhaTro = r.TenNhaTro,
            TenDuong = r.TenDuong,
            TenXa = r.TenXa,
            TenQuanHuyen = r.TenQuanhuyen,
            TrangThai = r.TrangThai,
            SoPhong = r.SoPhong,
            NgayTao = r.NgayTao,
            DiaChiDayDu = TaoDiaChiDayDu(r.SoNha, r.TenDuong, r.TenXa, r.TenQuanhuyen, r.ThanhPho, r.DiaChiChiTiet)
        }).ToList();
    }

    public async Task<NhaTroCreateUpdateViewModel?> GetForm(int? id, string userId)
    {
        var danhSachQuanHuyen = await GetQuanHuyenOptions();

        if (id is null or <= 0)
        {
            return new NhaTroCreateUpdateViewModel
            {
                TrangThai = TrangThaiMacDinh,
                DanhSachQuanHuyen = danhSachQuanHuyen
            };
        }

        var chuNhaTroId = await LayChuNhaTroIdTheoUser(userId);
        if (chuNhaTroId is null)
        {
            return null;
        }

        var row = await _context.NhaTros
            .AsNoTracking()
            .Where(n => n.Id == id.Value && n.ChuNhaTroId == chuNhaTroId.Value)
            .Select(n => new NhaTroCreateUpdateViewModel
            {
                Id = n.Id,
                TenNhaTro = n.TenNhaTro,
                MoTa = n.MoTa,
                SoNha = n.SoNha,
                DiaChiChiTiet = n.DiaChiChiTiet,
                DuongPhoId = n.DuongPhoId,
                ViDo = n.ViDo,
                KinhDo = n.KinhDo,
                TrangThai = n.TrangThai,
                XaId = n.DuongPho != null ? n.DuongPho.Xaid : null,
                QuanHuyenId = n.DuongPho != null ? n.DuongPho.Xa.Quanhuyenid : null
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }

        row.DanhSachQuanHuyen = danhSachQuanHuyen;
        row.DanhSachXa = await GetXaOptions(row.QuanHuyenId);
        row.DanhSachDuongPho = await GetDuongPhoOptions(row.XaId);
        return row;
    }

    public async Task<NhaTroRepositoryResult> CreateAsync(string userId, NhaTroCreateUpdateViewModel model)
    {
        var chuNhaTroId = await LayChuNhaTroIdTheoUser(userId);
        if (chuNhaTroId is null)
        {
            return ThatBai("Không xác định được hồ sơ chủ trọ.");
        }

        var trangThai = string.IsNullOrWhiteSpace(model.TrangThai) ? TrangThaiMacDinh : model.TrangThai.Trim();
        if (!TrangThaiNhaTroHopLe.Contains(trangThai))
        {
            return ThatBai("Trạng thái nhà trọ không hợp lệ.");
        }

        var (hopLeDuongPho, duongPhoId) = await XacThucDuongPhoId(model.DuongPhoId, model.XaId, model.QuanHuyenId);
        if (!hopLeDuongPho)
        {
            return ThatBai("Đường/phố không hợp lệ hoặc không khớp quận/phường đã chọn.");
        }

        var now = DateTime.Now;
        var nhaTro = new Domain.NhaTro
        {
            ChuNhaTroId = chuNhaTroId.Value,
            TenNhaTro = model.TenNhaTro.Trim(),
            MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? null : model.MoTa.Trim(),
            SoNha = string.IsNullOrWhiteSpace(model.SoNha) ? null : model.SoNha.Trim(),
            DiaChiChiTiet = model.DiaChiChiTiet.Trim(),
            DuongPhoId = duongPhoId,
            ViDo = model.ViDo,
            KinhDo = model.KinhDo,
            TrangThai = trangThai,
            NgayTao = now,
            NgayCapNhat = now
        };

        _context.NhaTros.Add(nhaTro);
        await _context.SaveChangesAsync();

        return ThanhCong("Thêm nhà trọ thành công.");
    }

    public async Task<NhaTroRepositoryResult> UpdateAsync(string userId, NhaTroCreateUpdateViewModel model)
    {
        if (model.Id <= 0)
        {
            return ThatBai("Mã nhà trọ không hợp lệ.");
        }

        var chuNhaTroId = await LayChuNhaTroIdTheoUser(userId);
        if (chuNhaTroId is null)
        {
            return ThatBai("Không xác định được hồ sơ chủ trọ.");
        }

        if (!TrangThaiNhaTroHopLe.Contains(model.TrangThai))
        {
            return ThatBai("Trạng thái nhà trọ không hợp lệ.");
        }

        var (hopLeDuongPho, duongPhoId) = await XacThucDuongPhoId(model.DuongPhoId, model.XaId, model.QuanHuyenId);
        if (!hopLeDuongPho)
        {
            return ThatBai("Đường/phố không hợp lệ hoặc không khớp quận/phường đã chọn.");
        }

        var nhaTro = await _context.NhaTros
            .FirstOrDefaultAsync(n => n.Id == model.Id && n.ChuNhaTroId == chuNhaTroId.Value);

        if (nhaTro is null)
        {
            return ThatBai("Không tìm thấy nhà trọ hoặc bạn không có quyền sửa.");
        }

        nhaTro.TenNhaTro = model.TenNhaTro.Trim();
        nhaTro.MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? null : model.MoTa.Trim();
        nhaTro.SoNha = string.IsNullOrWhiteSpace(model.SoNha) ? null : model.SoNha.Trim();
        nhaTro.DiaChiChiTiet = model.DiaChiChiTiet.Trim();
        nhaTro.DuongPhoId = duongPhoId;
        nhaTro.ViDo = model.ViDo;
        nhaTro.KinhDo = model.KinhDo;
        nhaTro.TrangThai = model.TrangThai.Trim();
        nhaTro.NgayCapNhat = DateTime.Now;

        await _context.SaveChangesAsync();

        return ThanhCong("Cập nhật nhà trọ thành công.");
    }

    public async Task<List<SelectListItem>> GetQuanHuyenOptions() =>
        await _context.Quanhuyens
            .AsNoTracking()
            .Where(q => q.TrangThai == TrangThaiHienThi)
            .OrderBy(q => q.TenQuanhuyen)
            .Select(q => new SelectListItem(q.TenQuanhuyen, q.Id.ToString()))
            .ToListAsync();

    public async Task<List<SelectListItem>> GetXaOptions(int? quanHuyenId)
    {
        if (quanHuyenId is null or <= 0)
        {
            return [];
        }

        return await _context.Xas
            .AsNoTracking()
            .Where(x => x.Quanhuyenid == quanHuyenId.Value && x.TrangThai == TrangThaiHienThi)
            .OrderBy(x => x.TenXahuyen)
            .Select(x => new SelectListItem(x.TenXahuyen, x.Id.ToString()))
            .ToListAsync();
    }

    public async Task<List<SelectListItem>> GetDuongPhoOptions(int? xaId)
    {
        if (xaId is null or <= 0)
        {
            return [];
        }

        return await _context.DuongPhos
            .AsNoTracking()
            .Where(d => d.Xaid == xaId.Value && d.TrangThai == TrangThaiHienThi)
            .OrderBy(d => d.TenDuong)
            .Select(d => new SelectListItem(d.TenDuong, d.Id.ToString()))
            .ToListAsync();
    }

    private async Task<int?> LayChuNhaTroIdTheoUser(string userId)
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

    private async Task<(bool IsValid, int? DuongPhoId)> XacThucDuongPhoId(
        int? duongPhoId,
        int? xaId,
        int? quanHuyenId)
    {
        if (duongPhoId is null or <= 0)
        {
            return (true, null);
        }

        var duongPho = await _context.DuongPhos
            .AsNoTracking()
            .Where(d => d.Id == duongPhoId.Value && d.TrangThai == TrangThaiHienThi)
            .Select(d => new { d.Id, d.Xaid, Quanhuyenid = d.Xa.Quanhuyenid })
            .FirstOrDefaultAsync();

        if (duongPho is null)
        {
            return (false, null);
        }

        if (xaId is > 0 && duongPho.Xaid != xaId.Value)
        {
            return (false, null);
        }

        if (quanHuyenId is > 0 && duongPho.Quanhuyenid != quanHuyenId.Value)
        {
            return (false, null);
        }

        return (true, duongPho.Id);
    }

    private static string TaoDiaChiDayDu(
        string? soNha,
        string? tenDuong,
        string? tenXa,
        string? tenQuanhuyen,
        string? thanhPho,
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

        if (!string.IsNullOrWhiteSpace(thanhPho))
        {
            parts.Add(thanhPho.Trim());
        }

        if (parts.Count > 0)
        {
            return string.Join(", ", parts);
        }

        return string.IsNullOrWhiteSpace(diaChiChiTiet)
            ? string.Empty
            : diaChiChiTiet.Trim();
    }

    private static bool TryParseNguoiDungId(string userId, out int nguoiDungId) =>
        int.TryParse(userId, out nguoiDungId) && nguoiDungId > 0;

    private static NhaTroRepositoryResult ThanhCong(string message) =>
        new() { Success = true, Message = message };

    private static NhaTroRepositoryResult ThatBai(string message) =>
        new() { Success = false, Message = message };
}
