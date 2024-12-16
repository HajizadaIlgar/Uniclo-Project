using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Brands;
using Ab108Uniqlo.ViewModels.Products;

namespace Ab108Uniqlo.ViewModels.Shops;

public class ShopVM
{
    public IEnumerable<BrandAndProductVM> Brands { get; set; }
    public IEnumerable<ProductListItemVM> Products { get; set; }
    public IEnumerable<ProductComment> ProductComments { get; set; }
    public int ProductCount { get; set; }
}
