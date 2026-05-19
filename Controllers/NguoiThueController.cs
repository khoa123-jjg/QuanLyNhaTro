using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaTro.Controllers;

[Authorize(Roles = "NGUOI_THUE")]
public class NguoiThueController : Controller
{
    [HttpGet]
    public IActionResult HoSo()
    {
        return View();
    }
}
