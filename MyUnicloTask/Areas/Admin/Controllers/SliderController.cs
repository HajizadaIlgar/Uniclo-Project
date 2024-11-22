using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Sliders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
public class SliderController(UnicloDbContext _contex, IWebHostEnvironment _env) : Controller
{
    Route["Slider"]
    public async Task<IActionResult> Index()
    {
        return View(await _contex.sliders.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(SliderCreateVM vm)
    {

        if (!ModelState.IsValid) return View(vm);
        if (!vm.File.ContentType.StartsWith("image"))
        {
            ModelState.AddModelError("File", "Sekilin olcusu saytimizin formatina uygun deyil");
            return View(vm);
        }
        if (vm.File.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("File", "Sekul yukle");
            return View(vm);
        }
        string newFileName = Path.GetRandomFileName() + Path.GetExtension(vm.File.FileName);
        using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "sliders", newFileName)))
        {
            await vm.File.CopyToAsync(stream);
        }
        Slider slider = new Slider
        {
            ImageUrl = newFileName,
            Title = vm.Title,
            Subtitle = vm.Subtitle,
            Link = vm.Link,
        };
        await _contex.sliders.AddAsync(slider);
        await _contex.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id, SliderCreateVM vm)
    {
        if (id is null) return BadRequest();
        var dats = _contex.sliders.Where(x => x.Id == id).FirstOrDefault();
        string filepath = Path.Combine(_env.WebRootPath, "imgs", "sliders");
        if (await _contex.sliders.AnyAsync(x => x.Id == id))
        {
            _contex.sliders.Remove(dats);
            _contex?.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}

