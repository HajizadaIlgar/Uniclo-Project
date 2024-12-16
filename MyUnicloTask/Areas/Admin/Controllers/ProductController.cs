using Ab108Uniqlo.Constant;
using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.Extensions;
using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Commons;
using Ab108Uniqlo.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
namespace Ab108Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RolesConstants.ControllerConst)]

public class ProductController(IWebHostEnvironment _env, UnicloDbContext _contex) : Controller
{
    public async Task<IActionResult> Index(int? page = 1, int? take = 4)
    {
        if (page == null) page = 1;
        if (!take.HasValue) take = 4;
        var query = _contex.Products.Include(x => x.Brand).AsQueryable();
        var data = await query.Skip(take.Value * page.Value - 1).Take(take.Value).ToListAsync();
        ViewBag.PaginationItems = new PaginationItemsVM(await query.CountAsync(), take.Value, page.Value);
        return View(data);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _contex.Brands.Where(x => !x.IsDeleted).ToListAsync();
        ViewBag.Currency = await _contex.Currencies.Where(x => !x.IsDeleted).ToListAsync();
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
            BrandId = vm.BrandId,
            CurrencyId = vm.CurrencyId,
        };

        if (vm.File != null)
        {
            product.CoverImage = await vm.File.UploadAsync(productImagePath);
        }

        if (vm.OtherFile != null)
        {
            product.Images = await Task.WhenAll(vm.OtherFile.Select(async x => new ProductImages
            {
                Products = product,
                ImageUrl = await x.UploadAsync(productImagePath)
            }));
        }

        await _contex.Products.AddAsync(product);
        await _contex.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    //public async Task<IActionResult> Update(int? id)
    //{
    //    ViewBag.Categories = await _contex.Brands.Where(x => !x.IsDeleted).ToListAsync();
    //    if (id == null) return BadRequest();
    //    var data = await _contex.Products.FindAsync(id.Value);
    //    if (data is null) return NotFound();
    //    return View(data);
    //}
    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();
        var data = await _contex.Products.Include(x => x.Images)
            .Where(x => x.Id == id)
           .Select(x => new ProductUpdateVM
           {
               Id = x.Id,
               Name = x.Name,
               SellPrice = x.SellPrice,
               Description = x.Description,
               CostPrice = x.CostPrice,
               BrandId = x.BrandId ?? 0,
               CurrencyId = x.CurrencyId ?? 0,
               FileUrl = x.CoverImage,
               Discount = x.Discount,
               Quantity = x.Quantity,
               OtherFilesUrl = x.Images.Select(x => x.ImageUrl)
           }).FirstOrDefaultAsync();
        if (data is null) return NotFound();
        ViewBag.Categories = await _contex.Brands.Where(x => !x.IsDeleted).ToListAsync();
        ViewBag.Currency = await _contex.Currencies.Where(x => !x.IsDeleted).ToListAsync();
        return View(data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int? id, ProductUpdateVM vm)
    { //burada elave sekili silenden sonra yeni sekil elave etmiyende null reference excpt verir 
        if (id == null) return BadRequest();
        var data = await _contex.Products
            .Include(x => x.Images)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
        if (data is null) return NotFound();
        if (vm.OtherFiles is not null)
        {
            data.Images.AddRange(vm.OtherFiles.Select(x => new ProductImages
            {
                ImageUrl = x.UploadAsync(Path.Combine(_env.WebRootPath, "imgs", "products"))
              .Result
            }).ToList());
        }
        if (vm.File is not null)
        {
            vm.Id = data.Id;
            vm.Name = data.Name;
            vm.SellPrice = data.SellPrice;
            vm.Description = data.Description;
            vm.CostPrice = data.CostPrice;
            vm.BrandId = data.BrandId ?? 0;
            vm.CurrencyId = data.CurrencyId ?? 0;
            vm.FileUrl = data.CoverImage;
            vm.Discount = data.Discount;
            vm.Quantity = data.Quantity;
            await _contex.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteImgs(int id, IEnumerable<string> imgnames)
    {
        int result = await _contex.ProductImage.Where(x => imgnames.Contains(x.ImageUrl)).ExecuteDeleteAsync();
        if (result == 0) return NotFound();
        if (result > 0)
        {
            var paths = Path.Combine(_env.WebRootPath, "imgs", "products").FirstOrDefault();

        }
        return RedirectToAction(nameof(Update), new { id });
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
