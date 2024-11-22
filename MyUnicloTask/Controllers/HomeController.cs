using Ab108Uniqlo.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.Controllers
{
    public class HomeController(UnicloDbContext _contex) : Controller
    {

        public async Task<IActionResult> Index()
        {
            return View(await _contex.sliders.ToListAsync());
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
