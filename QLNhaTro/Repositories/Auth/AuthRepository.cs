using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Domain;
using QLNhaTro.Helpers;
using QLNhaTro.Models.Auth;
using NguoiThueEntity = QLNhaTro.Domain.NguoiThue;

namespace QLNhaTro.Repositories.Auth;

public class AuthRepository : IAuthRepository
{
    private const string TrangThaiHoatDong = "HOAT_DONG";
    private const string VaiTroNguoiThue = "NGUOI_THUE";
    private const string VaiTroChuTro = "CHU_TRO";
    private const string VaiTroAdmin = "ADMIN";

    private readonly PhongTroDaNangContext _context;

    public AuthRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<AuthResult> Register(RegisterViewModel model)
    {
        var vaiTroDangKy = model.VaiTroDangKy.Trim().ToUpperInvariant();

        if (vaiTroDangKy == VaiTroAdmin)
        {
            return Fail("Không thể đăng ký tài khoản quản trị từ trang công khai.");
        }

        if (vaiTroDangKy is not (VaiTroNguoiThue or VaiTroChuTro))
        {
            return Fail("Vai trò đăng ký không hợp lệ. Chỉ được chọn Người thuê hoặc Chủ trọ.");
        }

        var email = model.Email.Trim();
        var soDienThoai = model.SoDienThoai.Trim();

        if (await _context.NguoiDungs.AnyAsync(u => u.Email == email))
        {
            return Fail("Email đã được sử dụng.");
        }

        if (await _context.NguoiDungs.AnyAsync(u => u.SoDienThoai == soDienThoai))
        {
            return Fail("Số điện thoại đã được sử dụng.");
        }

        var vaiTro = await _context.VaiTros
            .FirstOrDefaultAsync(v => v.TenVaiTro == vaiTroDangKy);

        if (vaiTro is null)
        {
            return Fail("Vai trò đăng ký chưa được cấu hình trong hệ thống.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {

            var nguoiDung = new NguoiDung
            {
                HoTen = model.HoTen.Trim(),
                Email = email,
                SoDienThoai = soDienThoai,
                MatKhauHash = PasswordHelper.HashPassword(model.MatKhau),
                TrangThai = TrangThaiHoatDong,
                NgayTao = DateTime.Now
            };

            _context.NguoiDungs.Add(nguoiDung);
            await _context.SaveChangesAsync();

            _context.NguoiDungVaiTros.Add(new NguoiDungVaiTro
            {
                NguoiDungId = nguoiDung.Id,
                VaiTroId = vaiTro.Id,
                NgayGan = DateTime.Now
            });

            if (vaiTroDangKy == VaiTroNguoiThue)
            {
                _context.NguoiThues.Add(new NguoiThueEntity
                {
                    NguoiDungId = nguoiDung.Id,
                    NgayTao = DateTime.Now
                });
            }
            else
            {
                _context.ChuNhaTros.Add(new ChuNhaTro
                {
                    NguoiDungId = nguoiDung.Id,
                    TrangThaiHoSo = TrangThaiHoatDong,
                    NgayTao = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new AuthResult
            {
                Success = true,
                Message = "Đăng ký tài khoản thành công.",
                NguoiDung = nguoiDung,
                Roles = [vaiTroDangKy]
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            return Fail("Đăng ký thất bại. Vui lòng thử lại sau.");
        }
    }

    public async Task<AuthResult> Login(LoginViewModel model)
    {
        var email = model.Email.Trim();

        var nguoiDung = await _context.NguoiDungs
            .Include(u => u.NguoiDungVaiTros)
                .ThenInclude(nv => nv.VaiTro)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (nguoiDung is null)
        {
            return Fail("Email hoặc mật khẩu không đúng.");
        }

        if (!string.Equals(nguoiDung.TrangThai, TrangThaiHoatDong, StringComparison.OrdinalIgnoreCase))
        {
            return Fail("Tài khoản đã bị khóa hoặc chưa được kích hoạt.");
        }

        if (!PasswordHelper.VerifyPassword(model.MatKhau, nguoiDung.MatKhauHash))
        {
            return Fail("Email hoặc mật khẩu không đúng.");
        }

        var roles = nguoiDung.NguoiDungVaiTros
            .Select(nv => nv.VaiTro.TenVaiTro)
            .ToList();

        return new AuthResult
        {
            Success = true,
            Message = "Đăng nhập thành công.",
            NguoiDung = nguoiDung,
            Roles = roles
        };
    }

    private static AuthResult Fail(string message) => new()
    {
        Success = false,
        Message = message
    };

}
