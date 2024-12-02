using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.ViewModels.Baskets;
using Ab108Uniqlo.ViewModels.Brands;
using Ab108Uniqlo.ViewModels.Products;
using Ab108Uniqlo.ViewModels.Shops;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace Ab108Uniqlo.Controllers;

public class ShopController(UnicloDbContext _contex) : Controller
{
    public async Task<IActionResult> Index(int? catId, string amount)
    {
        var query = _contex.Products.AsQueryable();
        if (catId.HasValue)
        {
            query = query.Where(x => x.BrandId == catId);
        }
        if (amount != null)
        {
            var prices = amount.Split(
                '-').Select(x => Convert.ToInt32(x));
            query = query.Where(y => prices.ElementAt(0) <= y.SellPrice && prices.ElementAt(1) >= y.SellPrice);
        }
        ShopVM vm = new ShopVM();
        vm.Brands = await _contex.Brands
            .Where(x => !x.IsDeleted)
            .Select(x => new BrandAndProductVM
            {
                Id = x.Id,
                Name = x.Name,
                Count = x.Products.Count
            }).ToListAsync();
        vm.Products = await query
            .Take(6)
            .Select(x => new ProductListItemVM
            {
                Id = x.Id,
                Name = x.Name,
                CoverImage = x.CoverImage,
                Discount = x.Discount,
                SellPrice = x.SellPrice,
                IsStock = x.Quantity > 0
            }).ToListAsync();
        return View(vm);
    }

    public async Task<IActionResult> AddBasket(int id)
    {
        var basket = getBasket();
        var item = basket.FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            item.Count++;
        }
        else
        {
            basket.Add(new BasketCookieItemVM
            {
                Id = id,
                Count = 1,
            });
        }
        string data = JsonSerializer.Serialize(basket);
        HttpContext.Response.Cookies.Append("basket", data);
        return Ok();
    }
    public async Task<IActionResult> GetBasket(int id)
    {

        return Json(getBasket());
    }
    List<BasketCookieItemVM> getBasket()
    {
        try
        {
            string? value = HttpContext.Request.Cookies["basket"];
            if (value is null) return new();
            return JsonSerializer.Deserialize<List<BasketCookieItemVM>>(value) ?? new();
        }
        catch (Exception)
        {

            return new();
        }
    }
}
