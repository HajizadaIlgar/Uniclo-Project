using Ab108Uniqlo.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ab108Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RolesConstants.ControllerConst)]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
