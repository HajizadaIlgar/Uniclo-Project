using Ab108Uniqlo.Models;

namespace Ab108Uniqlo.ViewModels.Shops
{
    public class DetailVM
    {
        public Product Product { get; set; }
        public IEnumerable<ProductComment> Comments { get; set; }
    }
}
