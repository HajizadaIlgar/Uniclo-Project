using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.Extensions;
using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        SelectList selectListItems = new SelectList(ViewBag.Categories, "Id", "Name");
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
            if (!vm.OtherFile.All(x => x.IsValidSize(400)))
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
            Description = vm.Description,
            Quantity = vm.Quantity,
            SellPrice = vm.SellPrice,
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

}
