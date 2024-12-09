using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.ViewModels.Commons;
using Ab108Uniqlo.ViewModels.Products;
using Ab108Uniqlo.ViewModels.Sliders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.Controllers
{
    public class HomeController(UnicloDbContext _contex) : Controller
    {

        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM();
            homeVM.Sliders = await _contex.Sliders
                .Select(x => new SliderListItemVM
                {
                    ImageUrl = x.ImageUrl,
                    Link = x.Link,
                    Subtitle = x.Subtitle,
                    Title = x.Title
                }).ToListAsync();
            homeVM.Brands = await _contex.Brands
                .OrderByDescending(x => x.Products!.Count)
                .Take(4)
                .ToListAsync();
            homeVM.PupolarProducts = await _contex.Products
                .Where(x => homeVM.Brands
                .Select(y => y.Id)
                .Contains(x.BrandId!.Value))
                .Take(10)
               .Select(x => new ProductListItemVM
               {
                   Id = x.Id,
                   Name = x.Name,
                   CoverImage = x.CoverImage,
                   Discount = x.Discount,
                   SellPrice = x.SellPrice,
                   IsStock = x.Quantity > 0,
                   BrandId = x.BrandId!.Value,
               })
               .ToListAsync();
            return View(homeVM);
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult AccesDenied()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        //public void SetSession(string key, string value)
        //{
        //    HttpContext.Session.SetString(key, value);
        //}
        //public IActionResult GetSession(string key)
        //{

        //    return Content(HttpContext.Session.GetString(key) ?? string.Empty);
        //}

        //public void SetCookie(string key, string value)
        //{
        //    HttpContext.Response.Cookies.Append(key, value, new CookieOptions
        //    {
        //        Expires = new DateTime(2024, 12, 02),
        //        //MaxAge = TimeSpan.FromMinutes(2)
        //    });
        //}
        //public IActionResult GetCookie(string key)
        //{

        //    return Content(HttpContext.Request.Cookies[key]);
        //}
        //public IActionResult RemoveCookie(string key)
        //{
        //    HttpContext.Response.Cookies.Delete(key);
        //    return Ok();
        //}
    }
}
