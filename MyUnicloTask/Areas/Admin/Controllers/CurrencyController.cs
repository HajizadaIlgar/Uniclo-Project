using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Currencies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.Areas.Admin.Controllers;
[Area("Admin")]
public class CurrencyController(UnicloDbContext _context) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _context.Currencies.ToListAsync());
    }
    public async Task<IActionResult> Create()
    {
        ViewBag.Currency = await _context.Currencies.Where(x => !x.IsDeleted).ToListAsync();
        SelectList sl = new SelectList(ViewBag.Currency, "Id", "Name");
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CurrencyCreateVM vm)
    {
        if (vm is null) return BadRequest();
        Currency c = new Currency
        {
            Id = vm.Id,
            Name = vm.Name,
        };
        await _context.Currencies.AddAsync(c);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return BadRequest();
        var data = await _context.Currencies.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (data is null) return BadRequest();
        _context.Currencies.Remove(data);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
