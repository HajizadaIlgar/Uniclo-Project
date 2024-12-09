using Ab108Uniqlo.Models;
using Ab108Uniqlo.ViewModels.Products;
using Ab108Uniqlo.ViewModels.Sliders;

namespace Ab108Uniqlo.ViewModels.Commons
{
    public class HomeVM
    {
        public IEnumerable<SliderListItemVM> Sliders { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<ProductListItemVM> PupolarProducts { get; set; }
    }
}
