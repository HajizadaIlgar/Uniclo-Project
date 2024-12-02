using Ab108Uniqlo.DataAccess;
using Ab108Uniqlo.Helpers;
using Ab108Uniqlo.ViewModels.Baskets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.ViewComponents
{
    public class LayoutHeaderViewComponent(UnicloDbContext _context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basket = BasketHelper.GetBasket(HttpContext.Request);
            var basketItems = await _context.Products
                  .Where(x => basket
                  .Select(x => x.Id)
                  .Contains(x.Id))
                  .Select(x => new BasketItemVM
                  {
                      Id = x.Id,
                      Name = x.Name,
                      ImgUrl = x.CoverImage,
                      SellPrice = x.SellPrice,
                      Discount = x.Discount,
                  })
                  .ToListAsync();
            foreach (var item in basketItems)
            {
                item.Count = basket.First(x => x.Id == item.Id).Count;
            }
            return View();
        }
    }
}
