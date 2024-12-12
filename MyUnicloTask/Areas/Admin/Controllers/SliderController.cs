using Ab108Uniqlo.Constant;
using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Commons;
using Ab108Uniqlo.ViewModels.Sliders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AB108Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RolesConstants.ControllerConst)]

public class SliderController(UnicloDbContext _context, IWebHostEnvironment _env) : Controller
{
    public async Task<IActionResult> Index(int? page = 1, int? take = 4)
    {
        ViewBag.Pagination = new PaginationItemsVM(await _context.Sliders.CountAsync(), take.Value, page.Value);
        return View(await _context.Sliders.Skip((page.Value - 1) * take.Value).Take(take.Value).ToListAsync());
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
            ModelState.AddModelError("File", "Format type must be image");
            return View(vm);
        }
        if (vm.File.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("File", "File size must be less than 2 mb");
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
            Subtitle = vm.Subtitle!,
            Link = vm.Link
        };
        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id is null) return BadRequest();
        var data = await _context.Sliders.FindAsync(id.Value);
        if (data == null) return NotFound();
        if (data != null) return View(data);

        return View(data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int? id, Slider data)
    {
        if (id is null) return BadRequest();

        var entity = _context.Sliders.Where(x => x.Id == id).FirstOrDefault();
        if (entity is null) return NotFound();
        if (entity != null)
        {
            entity.Title = data.Title;
            entity.Subtitle = data.Subtitle;
            entity.Link = data.Link;
            entity.ImageUrl = data.ImageUrl;
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int? id, SliderCreateVM vm)
    {
        if (id is null) return BadRequest();
        var dats = _context.Sliders.Where(x => x.Id == id).FirstOrDefault();
        string filepath = Path.Combine(_env.WebRootPath, "imgs", "sliders");
        if (await _context.Sliders.AnyAsync(x => x.Id == id))
        {
            _context.Sliders.Remove(dats);
            await _context?.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Toggle(int? id)
    {
        if (id is null) return BadRequest();
        var dats = await _context.Sliders
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
        if (dats is null) return NotFound();
        dats.IsDeleted = !dats.IsDeleted;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
