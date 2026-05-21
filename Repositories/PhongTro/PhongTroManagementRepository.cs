using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Domain;
using QLNhaTro.Models.PhongTro;
using QuanLyNhaTro.Helpers.Constants;
using QuanLyNhaTro.Models.TienNghi;
namespace QLNhaTro.Repositories.PhongTro;

public class PhongTroManagementRepository : IPhongTroManagementRepository
{
    private static readonly Dictionary<string, string> IconSlugMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["wifi"] = "wifi",
        ["air-conditioner"] = "snow",
        ["parking"] = "bicycle",
        ["washing-machine"] = "droplet-half",
        ["toilet"] = "droplet",
        ["loft"] = "layers",
        ["camera"] = "camera-video",
        ["clock"] = "clock"
    };

    private readonly PhongTroDaNangContext _context;

    public PhongTroManagementRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }
    // Lấy danh sách nhà trọ của chủ trọ để hiển thị trong dropdown chọn nhà trọ khi tìm kiếm được đưa lên controller để dùng
    public async Task<List<SelectListItem>> GetDanhSachNhaTroCuaChuTroAsync(string userId)
    {
        // userId do hệ thống xác thực cung cấp thường là kiểu chuỗi, cần trả về int để truy vấn
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return [];
        }
        //Truy vấn danh sách nhà trọ lấy kèm id để xử lý khi chon option đưa lên database
        return await _context.NhaTros
            .AsNoTracking()
            .Where(n => n.ChuNhaTro.NguoiDungId == nguoiDungId)
            .OrderBy(n => n.TenNhaTro)
            .Select(n => new SelectListItem(n.TenNhaTro, n.Id.ToString()))// kèm id để khi chọn option sẽ lấy được id đưa lên database
            .ToListAsync();
    }
    // Lấy danh sách phòng theo tìm kiếm và lọc, nếu có, và lấy dữ liệu để đổ vào dropdown chọn nhà trọ, tầng, trạng thái khi quản lý phòng
    public async Task<PhongTroListPageViewModel> GetDanhSachPhongAsync(
        string userId,
        string? tuKhoa,
        int? nhaTroId,
        int? tang,
        string? trangThai)
    {
        var page = new PhongTroListPageViewModel
        {
            TuKhoa = tuKhoa,
            NhaTroId = nhaTroId,
            Tang = tang,
            TrangThai = trangThai
        };
        // userId do hệ thống xác thực cung cấp thường là kiểu chuỗi, cần trả về int để truy vấn
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return page;
        }
        // Lấy danh sách nhà trọ của chủ trọ để hiển thị trong dropdown chọn nhà trọ khi quản lý phòng
        page.DanhSachNhaTro = await GetDanhSachNhaTroCuaChuTroAsync(userId);

        var tangValues = await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Where(p => p.Tang != null)
            .Select(p => p.Tang!.Value)// ép kiểu int
            .Distinct()// Loại bỏ trùng lặp nếu có 5 phòng ở tầng 1 thì chỉ lấy giá trị 1 một lần nếu không sẽ có 5 option tầng 1 trong dropdown
            .OrderBy(t => t)
            .ToListAsync();

            page.DanhSachTang = tangValues
            .Select(t => new SelectListItem($"Tầng {t}", t.ToString()))
            .ToList();

        var query = _context.PhongTros
            .AsNoTracking()
            .Where(p => p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId);
        //nhaTroId nếu có thì thêm vào truy vấn, không có thì bỏ qua
        if (nhaTroId is > 0)
        {
            query = query.Where(p => p.NhaTroId == nhaTroId.Value);
        }
        //tang nếu có thì thêm vào truy vấn, không có thì bỏ qua
        if (tang.HasValue)
        {
            query = query.Where(p => p.Tang == tang.Value);
        }
        //trangThai là các điều kiện lọc, nếu có thì thêm vào truy vấn, không có thì bỏ qua
        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            //Helper/Constants/PhongTroStatus là Helper dùng chung định nghĩa các trạng thái của phòng trọ được lưu trong Constants
            var trangThaiLoc = trangThai.Trim();
            if (PhongTroStatus.IsValid(trangThaiLoc))
            {
                query = query.Where(p => p.TrangThai == trangThaiLoc);
            }
        }

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(p =>
                p.MaPhong.Contains(keyword) ||
                p.TenPhong.Contains(keyword) ||
                (p.GhiChu != null && p.GhiChu.Contains(keyword)));
        }
        //Bắt đầu đưa ra kết quả sau khi đã lọc xong, sắp xếp theo tên nhà trọ, tầng, mã phòng
        page.DanhSachPhong = await query
            .OrderBy(p => p.NhaTro.TenNhaTro)
            .ThenBy(p => p.Tang)
            .ThenBy(p => p.MaPhong)
            .Select(p => new PhongTroListItemViewModel
            {
                Id = p.Id,
                MaPhong = p.MaPhong,
                TenPhong = p.TenPhong,
                TenNhaTro = p.NhaTro.TenNhaTro,
                Tang = p.Tang,
                DienTich = p.DienTich,
                GiaThueThang = p.GiaThueThang,
                TienCoc = p.TienCoc,
                SoNguoiToiDa = p.SoNguoiToiDa,
                TrangThai = p.TrangThai,
                GhiChu = p.GhiChu,
                MoTa = p.MoTa
            })
            .ToListAsync();

        return page;
    }
    // Chỉ trả về form không hộ trọ nhập dữ liệu hay cập nhật
    public async Task<PhongTroCreateUpdateViewModel?> GetPhongFormAsync(int? id, string userId)
    {
        // Lấy danh sách nhà trọ của chủ trọ để hiển thị trong dropdown sửa nếu có id,  không thì thêm mới
        var danhSachNhaTro = await GetDanhSachNhaTroCuaChuTroAsync(userId);
        // Trường hợp không có id
        if (id is null or <= 0)
        {
            return new PhongTroCreateUpdateViewModel
            {
                TrangThai = PhongTroStatus.MacDinh,
                // Đưa danh sách nhà trọ vào để hiển thị trong dropdown chọn nhà trọ khi quản lý phòng
                DanhSachNhaTro = danhSachNhaTro
            };
        }
        //Không tìm thấy nhà trọ nào của chủ trọ
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
        // Lấy danh sách nhà trọ của chủ trọ để hiển thị trong dropdown sửa nếu có id,  không thì thêm mới
        row.DanhSachNhaTro = danhSachNhaTro;
        return row;
    }

    private static PhongTroManagementResult ThanhCong(string message) =>
        new() { Success = true, Message = message };

    private static PhongTroManagementResult ThatBai(string message) =>
        new() { Success = false, Message = message };
    // Hỗ trợ cho việc thêm mới và cập nhật phòng, biết chủ nhà trọ nào đang thêm phòng hoặc cập nhật
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
    //Tạo phòng
    public async Task<PhongTroManagementResult> CreatePhong(string userId, PhongTroCreateUpdateViewModel model)
    {
        var chuNhaTroId = await LayChuNhaTroIdTheoNguoiDung(userId);
        //asp-validation-summary
        if (chuNhaTroId is null)
        {
            return ThatBai("Không xác định được hồ sơ chủ trọ.");
        }
        //asp-validation-summary
        //Helper/Constants/PhongTroStatus là Helper dùng chung định nghĩa các trạng thái của phòng trọ được lưu trong Constants
        if (!PhongTroStatus.IsValid(model.TrangThai))
        {
            return ThatBai("Trạng thái phòng không hợp lệ.");
        }
        //asp-validation-summary
        if (!await NhaTroThuocChu(chuNhaTroId.Value, model.NhaTroId))
        {
            return ThatBai("Nhà trọ không thuộc quyền quản lý của bạn.");
        }
        //asp-validation-summary
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
    //Sửa phòng
    public async Task<PhongTroManagementResult> UpdatePhongAsync(string userId,PhongTroCreateUpdateViewModel model)
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
        //Helper/Constants/PhongTroStatus là Helper dùng chung định nghĩa các trạng thái của phòng trọ được lưu trong Constants
        if (!PhongTroStatus.IsValid(model.TrangThai))
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

    public async Task<TienNghiPhongPageViewModel?> GetGanTienNghiTrangAsync(int? phongTroId, string userId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return null;
        }

        var phongRows = await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .OrderBy(p => p.NhaTro.TenNhaTro)
            .ThenBy(p => p.MaPhong)
            .Select(p => new
            {
                p.Id,
                p.MaPhong,
                p.TenPhong,
                p.DienTich,
                p.SoNguoiToiDa,
                TenNhaTro = p.NhaTro.TenNhaTro
            })
            .ToListAsync();

        var danhSachPhong = phongRows
            .Select(p => new SelectListItem(
                $"{p.MaPhong} - {p.TenNhaTro}",
                p.Id.ToString()))
            .ToList();

        var catalog = await LayDanhSachTienNghiHienThiAsync();

        var model = new TienNghiPhongPageViewModel
        {
            PhongTroIdDangChon = phongTroId,
            DanhSachPhong = danhSachPhong,
            DanhSachTienNghi = catalog
        };

        if (phongRows.Count == 0 || phongTroId is null or <= 0)
        {
            return model;
        }

        if (!phongRows.Any(p => p.Id == phongTroId.Value))
        {
            model.PhongTroIdDangChon = null;
            return model;
        }

        var daChonIds = await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.Id == phongTroId.Value && p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .SelectMany(p => p.TienNghis.Select(t => t.Id))
            .ToListAsync();

        var daChonSet = daChonIds.ToHashSet();
        foreach (var item in model.DanhSachTienNghi)
        {
            item.DaChon = daChonSet.Contains(item.Id);
        }

        model.TienNghiDaChon = model.DanhSachTienNghi
            .Where(t => t.DaChon)
            .ToList();

        return model;
    }

    private async Task<List<TienNghiItemViewModel>> LayDanhSachTienNghiHienThiAsync()
    {
        var rows = await _context.TienNghis
            .AsNoTracking()
            .Where(t => t.TrangThai == DisplayStatus.HienThi)
            .OrderBy(t => t.TenTienNghi)
            .Select(t => new { t.Id, t.TenTienNghi, t.Icon })
            .ToListAsync();

        return rows.Select(t => new TienNghiItemViewModel
        {
            Id = t.Id,
            TenTienNghi = t.TenTienNghi,
            Icon = TaoIconBootstrap(t.Icon)
        }).ToList();
    }

    private static string TaoIconBootstrap(string? icon)
    {
        if (string.IsNullOrWhiteSpace(icon))
        {
            return "bi bi-check-circle";
        }

        var slug = icon.Trim();
        if (slug.Contains(' ', StringComparison.Ordinal))
        {
            return slug.StartsWith("bi", StringComparison.OrdinalIgnoreCase) ? slug : $"bi {slug}";
        }

        if (slug.StartsWith("bi-", StringComparison.OrdinalIgnoreCase))
        {
            return $"bi {slug}";
        }

        if (IconSlugMap.TryGetValue(slug, out var mapped))
        {
            return $"bi bi-{mapped}";
        }

        return $"bi bi-{slug}";
    }

    private static bool TryParseNguoiDungId(string userId, out int nguoiDungId) =>
        int.TryParse(userId, out nguoiDungId) && nguoiDungId > 0;
}
