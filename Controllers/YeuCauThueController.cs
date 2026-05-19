using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaTro.Controllers;

[Authorize(Roles = "NGUOI_THUE")]
public class YeuCauThueController : Controller
{
    [HttpGet]
    public IActionResult GuiYeuCau()
    {
        return View();
    }

    [HttpGet]
    public IActionResult DanhSach()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ChiTiet()
    {
        return View();
    }

    [HttpGet]
    public IActionResult HuyYeuCau()
    {
        return View();
    }
}
