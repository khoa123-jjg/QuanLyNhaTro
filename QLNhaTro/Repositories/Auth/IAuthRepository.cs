using QLNhaTro.Domain;
using QLNhaTro.Models.Auth;

namespace QLNhaTro.Repositories.Auth;

public class AuthResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public NguoiDung? NguoiDung { get; set; }

    public List<string> Roles { get; set; } = [];
}

public interface IAuthRepository
{
    Task<AuthResult> Register(RegisterViewModel model);

    Task<AuthResult> Login(LoginViewModel model);
}
