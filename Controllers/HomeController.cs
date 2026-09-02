using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Models;
using TechnifyERP.Application.Website;

namespace TechnifyERP.Controllers;

public class HomeController(IHomepageService homepageService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) => View(await homepageService.GetAsync(cancellationToken));

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
