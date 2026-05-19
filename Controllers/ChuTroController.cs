using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaTro.Controllers;

[Authorize(Roles = "CHU_TRO")]
public class ChuTroController : Controller
{
    [HttpGet]
    public IActionResult TongQuan()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ThemSuaNhaTro()
    {
        return View();
    }

    [HttpGet]
    public IActionResult QuanLyDiaChi()
    {
        return View();
    }

    [HttpGet]
    public IActionResult LoaiPhong()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ThemSuaLoaiPhong()
    {
        return View();
    }

    [HttpGet]
    public IActionResult TienNghi()
    {
        return View();
    }

    [HttpGet]
    public IActionResult HinhAnh()
    {
        return View();
    }

    [HttpGet]
    public IActionResult PhongCuThe()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ThemSuaPhongCuThe()
    {
        return View();
    }

    [HttpGet]
    public IActionResult BaiDang()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ThemSuaBaiDang()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GuiBaiChoDuyet()
    {
        return View();
    }

    [HttpGet]
    public IActionResult YeuCauThue()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ChiTietYeuCauThue()
    {
        return View();
    }

    [HttpGet]
    public IActionResult XuLyYeuCauThue()
    {
        return View();
    }

    [HttpGet]
    public IActionResult CaiDat()
    {
        return View();
    }
}
