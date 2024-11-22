using Microsoft.AspNetCore.Mvc;

namespace Ab108Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
