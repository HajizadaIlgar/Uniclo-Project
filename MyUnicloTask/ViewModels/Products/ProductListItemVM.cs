using Ab108Uniqlo.Models;

namespace Ab108Uniqlo.ViewModels.Products
{
    public class ProductListItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal SellPrice { get; set; }
        public int Discount { get; set; }
        public bool IsStock { get; set; }
        public string CoverImage { get; set; }
        public int BrandId { get; set; }
        public int CurrencyId { get; set; }
        public ICollection<ProductImages>? Images { get; set; }

    }
}
