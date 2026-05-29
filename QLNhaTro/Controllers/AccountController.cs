using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNhaTro.Models.Auth;
using QLNhaTro.Repositories.Auth;

namespace QLNhaTro.Controllers;

public class AccountController : Controller
{
    private const string VaiTroAdmin = "ADMIN";
    private const string VaiTroChuTro = "CHU_TRO";
    private const string VaiTroNguoiThue = "NGUOI_THUE";

    private readonly IAuthRepository _authRepository;

    public AccountController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            return RedirectAfterLogin(roles, returnUrl);
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authRepository.Login(model);

        if (!result.Success || result.NguoiDung is null)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.NguoiDung.Id.ToString()),
            new(ClaimTypes.Name, result.NguoiDung.HoTen),
            new(ClaimTypes.Email, result.NguoiDung.Email),
        };

        foreach (var role in result.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = model.GhiNhoDangNhap
            });

        return RedirectAfterLogin(result.Roles, returnUrl);
    }

    private IActionResult RedirectAfterLogin(IEnumerable<string> roles, string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        var roleSet = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (roleSet.Contains(VaiTroAdmin))
        {
            return RedirectToAction("Dashboard", "Admin");
        }

        if (roleSet.Contains(VaiTroChuTro))
        {
            return RedirectToAction("PhongCuThe", "ChuTro");
        }

        if (roleSet.Contains(VaiTroNguoiThue))
        {
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authRepository.Register(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return Content("Bạn không có quyền truy cập");
    }
}
