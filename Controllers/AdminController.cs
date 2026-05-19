using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaTro.Controllers;

[Authorize(Roles = "ADMIN")]
public class AdminController : Controller
{
    [HttpGet]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public IActionResult NguoiDung()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ChiTietNguoiDung()
    {
        return View();
    }

    [HttpGet]
    public IActionResult KhoaMoKhoaTaiKhoan()
    {
        return View();
    }

    [HttpGet]
    public IActionResult BaiChoDuyet()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ChiTietBaiChoDuyet()
    {
        return View();
    }

    [HttpGet]
    public IActionResult DuyetBai()
    {
        return View();
    }

    [HttpGet]
    public IActionResult TuChoiBai()
    {
        return View();
    }

    [HttpGet]
    public IActionResult TienNghi()
    {
        return View();
    }

    [HttpGet]
    public IActionResult DonViHanhChinh()
    {
        return View();
    }

    [HttpGet]
    public IActionResult DuongPho()
    {
        return View();
    }

    [HttpGet]
    public IActionResult HoSo()
    {
        return View();
    }
}
