using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.Extensions;
using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Ab108Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController(IWebHostEnvironment _env, UnicloDbContext _contex) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _contex.Products.Include(x => x.Brand).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _contex.Brands.Where(x => !x.IsDeleted).ToListAsync();
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {
        if (vm.File != null)
        {
            if (!vm.File.IsValidType("image"))
                ModelState.AddModelError("File", "File must be an image");
            if (!vm.File.IsValidSize(400))
                ModelState.AddModelError("File", "File must be less than 400kb");
        }

        if (vm.OtherFile != null && vm.OtherFile.Any())
        {
            if (!vm.OtherFile.All(x => x.IsValidType("image")))
            {
                string fileNames = string.Join(',', vm.OtherFile.Where(x => !x.IsValidType("image")).Select(x => x.FileName));
                ModelState.AddModelError("OtherFiles", fileNames + " is (are) not an image");
            }
            if (!vm.OtherFile.All(x => x.IsValidSize(1000)))
            {
                string fileNames = string.Join(',', vm.OtherFile.Where(x => !x.IsValidSize(400)).Select(x => x.FileName));
                ModelState.AddModelError("OtherFiles", fileNames + " is (are) bigger than 400kb");
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _contex.Brands.Where(x => !x.IsDeleted).ToListAsync();
            return View(vm);
        }

        if (!await _contex.Brands.AnyAsync(x => x.Id == vm.BrandId && !x.IsDeleted))
        {
            ViewBag.Categories = await _contex.Brands.Where(x => !x.IsDeleted).ToListAsync();
            ModelState.AddModelError("BrandId", "Brand not found or deleted");
            return View(vm);
        }

        var productImagePath = Path.Combine(_env.WebRootPath, "imgs", "products");
        if (!Directory.Exists(productImagePath))
        {
            Directory.CreateDirectory(productImagePath);
        }

        Product product = new Product
        {
            Name = vm.Name,
            CoverImage = productImagePath,
            Description = vm.Description,
            Quantity = vm.Quantity,
            SellPrice = vm.SellPrice,
            CostPrice = vm.CostPrice,
            Discount = vm.Discount,
            BrandId = vm.BrandId
        };

        if (vm.File != null)
        {
            product.CoverImage = await vm.File.UploadAsync(productImagePath);
        }

        if (vm.OtherFile != null)
        {
            product.Images = await Task.WhenAll(vm.OtherFile.Select(async x => new ProductImage
            {
                Products = product,
                ImageUrl = await x.UploadAsync(productImagePath)
            }));
        }

        await _contex.Products.AddAsync(product);
        await _contex.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        ViewBag.Categories = await _contex.Brands.Where(x => !x.IsDeleted).ToListAsync();
        if (id == null) return BadRequest();
        var data = await _contex.Products.FindAsync(id.Value);
        if (data is null) return NotFound();
        return View(data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int? id, ProductCreateVM product)
    {
        if (id == null) return BadRequest();
        var data = await _contex.Products.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (data is null) return NotFound();
        if (data != null)
        {
            product.Name = data.Name;
            product.Description = data.Description;
            product.Discount = data.Discount;
            product.CostPrice = data.CostPrice;
            product.SellPrice = data.SellPrice;
            product.Quantity = data.Quantity;
            await _contex.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        dynamic dats = await _contex.Products.FindAsync(id.Value);
        if (dats is null) return NotFound();
        _contex.Products.Remove(dats);
        await _contex.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
