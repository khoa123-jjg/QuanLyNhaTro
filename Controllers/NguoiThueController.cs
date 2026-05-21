using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QLNhaTro.Controllers;

[Authorize(Roles = "NGUOI_THUE")]
public class NguoiThueController : Controller
{
    [HttpGet]
    public IActionResult HoSo()
    {
        return View();
    }
}
