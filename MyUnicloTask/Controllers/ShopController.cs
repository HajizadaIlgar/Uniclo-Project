﻿using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Baskets;
using Ab108Uniqlo.ViewModels.Brands;
using Ab108Uniqlo.ViewModels.Products;
using Ab108Uniqlo.ViewModels.Shops;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        return RedirectToAction("Index", "Shop");
    }

    public async Task<IActionResult> GetBasket(int id)
    {

        return Json(getBasket());
    }
    public async Task<IActionResult> Detail(int? id)
    {
        if (id is null) return BadRequest();
        var datas = await _contex.Products.Include(x => x.Images).Include(x => x.ProductRatings)
            .Where(s => s.Id == id.Value && !s.IsDeleted).FirstOrDefaultAsync();
        if (datas is null) return NotFound();
        string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is not null)
        {
            var rating = await _contex.ProductRatings.Where(x => x.UserId == userId && x.ProductId == id).Select(x => x.RatingRate).FirstOrDefaultAsync();
            ViewBag.Rating = rating == 0 ? 5 : rating;
        }
        else
        {
            ViewBag.Rating = 5;
        }
        await _contex.SaveChangesAsync();
        DetailVM vm = new DetailVM();
        vm.Product = datas;
        vm.Comments = await _contex.ProductComments.Where(x => x.ProductId == id).ToListAsync();
        return View(vm);
    }
    public async Task<IActionResult> Rate(int? productId, int rate = 1)
    {
        if (!productId.HasValue) return BadRequest();
        string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
        if (!await _contex.Products.AnyAsync(s => s.Id == productId)) return NotFound();
        var rating = await _contex.ProductRatings.Where(x => x.ProductId == productId && x.UserId == userId).FirstOrDefaultAsync();
        if (rating is null)
        {
            await _contex.AddAsync(new Models.ProductRating
            {
                ProductId = productId.Value,
                UserId = userId,
                RatingRate = rate
            });
        }
        else
        {
            rating.RatingRate = rate;
        }
        await _contex.SaveChangesAsync();
        return RedirectToAction(nameof(Detail), new { id = productId });
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
    public async Task<IActionResult> BasketDelete(int? id)
    {
        var basket = getBasket();
        var data = basket.FirstOrDefault(x => x.Id == id);
        if (data is null) return BadRequest();
        basket.Remove(data);
        var updateBasket = JsonSerializer.Serialize(basket);
        HttpContext.Response.Cookies.Append("basket", updateBasket);
        if (basket.Any())
        {
            HttpContext.Response.Cookies.Delete(updateBasket);
        }
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> CommentIndex(int? id)
    {
        return View(await _contex.ProductComments.Where(x => x.ProductId == id).ToListAsync());
    }
    public async Task<IActionResult> CommentCreate()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CommentCreate(int? productId, string commentText)
    {

        if (!productId.HasValue) return BadRequest();
        string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
        if (!await _contex.Products.AnyAsync(p => p.Id == productId)) return NotFound();

        await _contex.ProductComments.AddAsync(new ProductComment
        {
            CreatedTime = DateTime.Now,
            IsDeleted = false,
            UserId = userId,
            ProductId = productId.Value,
            Content = commentText,
        });
        await _contex.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> CommentDelete(int? id)
    {
        if (id == null) return BadRequest();
        var data = await _contex.ProductComments.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (data is null) return BadRequest();
        _contex.Remove(data);
        await _contex.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
