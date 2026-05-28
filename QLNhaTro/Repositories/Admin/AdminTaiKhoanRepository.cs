using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Helpers;
using QLNhaTro.Models.Admin.HoSo;

namespace QLNhaTro.Repositories.Admin;

public class AdminTaiKhoanRepository : IAdminTaiKhoanRepository
{
    private readonly PhongTroDaNangContext _context;

    public AdminTaiKhoanRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<AdminHoSoViewModel?> GetHoSoAsync(string userId)
    {
        if (!int.TryParse(userId, out var nguoiDungId))
        {
            return null;
        }

        var nguoiDung = await _context.NguoiDungs
            .AsNoTracking()
            .Include(x => x.NguoiDungVaiTros)
                .ThenInclude(x => x.VaiTro)
            .FirstOrDefaultAsync(x => x.Id == nguoiDungId);

        if (nguoiDung is null)
        {
            return null;
        }

        var vaiTro = nguoiDung.NguoiDungVaiTros
            .Select(x => x.VaiTro.TenVaiTro)
            .FirstOrDefault() ?? "Quản trị viên";

        return new AdminHoSoViewModel
        {
            HoTen = nguoiDung.HoTen,
            Email = nguoiDung.Email,
            SoDienThoai = nguoiDung.SoDienThoai ?? string.Empty,
            VaiTro = vaiTro,
            NgayTao = nguoiDung.NgayTao,
            GhiChu = nguoiDung.GhiChu,
            AnhDaiDien = nguoiDung.AnhDaiDien
        };
    }

    public async Task<(bool Success, string Message)> DoiMatKhauAsync(string userId, AdminDoiMatKhauViewModel model)
    {
        if (!int.TryParse(userId, out var nguoiDungId))
        {
            return (false, "Không tìm thấy tài khoản.");
        }

        var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Id == nguoiDungId);
        if (nguoiDung is null)
        {
            return (false, "Không tìm thấy tài khoản.");
        }

        if (!PasswordHelper.VerifyPassword(model.MatKhauHienTai, nguoiDung.MatKhauHash))
        {
            return (false, "Mật khẩu hiện tại không đúng.");
        }

        if (model.MatKhauMoi != model.XacNhanMatKhauMoi)
        {
            return (false, "Xác nhận mật khẩu không khớp.");
        }

        if (model.MatKhauMoi.Length < 6)
        {
            return (false, "Mật khẩu mới phải có ít nhất 6 ký tự.");
        }

        nguoiDung.MatKhauHash = PasswordHelper.HashPassword(model.MatKhauMoi);
        nguoiDung.NgayCapNhat = DateTime.Now;

        await _context.SaveChangesAsync();
        return (true, "Đổi mật khẩu thành công.");
    }

    public async Task<(bool Success, string Message)> CapNhatHoSoAsync(string userId, AdminHoSoViewModel model)
    {
        if (!int.TryParse(userId, out var nguoiDungId))
        {
            return (false, "Không tìm thấy tài khoản.");
        }

        var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Id == nguoiDungId);
        if (nguoiDung is null)
        {
            return (false, "Không tìm thấy tài khoản.");
        }

        var hoTen = model.HoTen.Trim();
        var email = model.Email.Trim();
        var soDienThoai = model.SoDienThoai?.Trim();

        if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(email))
        {
            return (false, "Vui lòng nhập đầy đủ thông tin bắt buộc.");
        }

        var emailDaTonTai = await _context.NguoiDungs.AnyAsync(x => x.Email == email && x.Id != nguoiDungId);
        if (emailDaTonTai)
        {
            return (false, "Email đã được sử dụng bởi tài khoản khác.");
        }

        if (!string.IsNullOrWhiteSpace(soDienThoai))
        {
            var sdtDaTonTai = await _context.NguoiDungs.AnyAsync(x => x.SoDienThoai == soDienThoai && x.Id != nguoiDungId);
            if (sdtDaTonTai)
            {
                return (false, "Số điện thoại đã được sử dụng bởi tài khoản khác.");
            }
        }

        nguoiDung.HoTen = hoTen;
        nguoiDung.Email = email;
        nguoiDung.SoDienThoai = soDienThoai;
        nguoiDung.GhiChu = model.GhiChu?.Trim();
        nguoiDung.NgayCapNhat = DateTime.Now;

        await _context.SaveChangesAsync();
        return (true, "Cập nhật hồ sơ thành công.");
    }
}
