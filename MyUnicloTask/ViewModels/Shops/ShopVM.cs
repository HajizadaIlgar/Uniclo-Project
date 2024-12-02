using Ab108Uniqlo.ViewModels.Brands;
using Ab108Uniqlo.ViewModels.Products;

namespace Ab108Uniqlo.ViewModels.Shops;

public class ShopVM
{
    public IEnumerable<BrandAndProductVM> Brands { get; set; }
    public IEnumerable<ProductListItemVM> Products { get; set; }
    public IEnumerable<ProductListItemVM> ProductCount { get; set; }
}
