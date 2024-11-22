using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.ViewModels.Sliders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {

        readonly UnicloDbContext _context;
        public SliderController(UnicloDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.sliders.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(SliderCreateVM vm)
        {

            if (!ModelState.IsValid) return View(vm);
            if (!vm.File.ContentType.StartsWith("images"))
            {
                ModelState.AddModelError("File", "Sekul yukle");
                return View();
            }
            if (vm.File.Length > 2 * 1024 * 2)
            {
                ModelState.AddModelError("File", "Sekilin olcusu saytimizin formatina uygun deyil");
            }

            return View();
        }
    }
}
