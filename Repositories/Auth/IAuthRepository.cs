using QuanLyNhaTro.Domain;
using QuanLyNhaTro.Models.Auth;

namespace QuanLyNhaTro.Repositories.Auth;

public class AuthResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public NguoiDung? NguoiDung { get; set; }

    public List<string> Roles { get; set; } = [];
}

public interface IAuthRepository
{
    Task<AuthResult> RegisterAsync(RegisterViewModel model);

    Task<AuthResult> LoginAsync(LoginViewModel model);
}
