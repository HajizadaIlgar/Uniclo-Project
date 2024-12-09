using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Brands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize]
public class BrandController(UnicloDbContext _contex, IWebHostEnvironment _env) : Controller
{
    public IActionResult Index()
    {
        var brands = _contex.Brands.ToList();
        return View(brands);
    }
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _contex.Brands.Where(x => !x.IsDeleted).ToListAsync();
        SelectList selectListItems = new SelectList(ViewBag.Categories, "Id", "Name");
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(BrandCreateVM vm)
    {
        if (vm.Name == null) return NotFound();
        Brand brand = new Brand
        {
            Name = vm.Name,
        };
        await _contex.Brands.AddAsync(brand);
        await _contex.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();
        var items = await _contex.Brands.FindAsync(id.Value);
        if (items == null) return NotFound();
        if (items != null) return View(items);
        return View(items);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int? id, string name, Brand items)
    {
        if (id is null) return BadRequest();
        var datas = await _contex.Brands.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (datas == null) return NotFound();
        if (datas is not null)
        {
            datas.Name = items.Name;
            _contex.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id, BrandCreateVM data)
    {
        if (id is null) return BadRequest();
        var searc_id = await _contex.Brands.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (await _contex.Brands.AnyAsync(x => x.Id == id))
        {
            _contex.Brands.Remove(searc_id);
            await _contex.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

}
