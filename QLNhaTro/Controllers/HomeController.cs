using Microsoft.AspNetCore.Mvc;
using QLNhaTro.Models;
using QLNhaTro.Models.Home;
using QLNhaTro.Repositories.PhongTro;
using System.Diagnostics;

namespace QLNhaTro.Controllers;

public class HomeController : Controller
{
    private readonly IPhongTroRepository _phongTroRepository;

    public HomeController(IPhongTroRepository phongTroRepository)
    {
        _phongTroRepository = phongTroRepository;
    }

    public async Task<IActionResult> Index()
    {
        var phongNoiBat = await _phongTroRepository.LayPhongNoiBatAsync(4);

        var model = new HomeIndexViewModel
        {
            PhongNoiBat = phongNoiBat
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
