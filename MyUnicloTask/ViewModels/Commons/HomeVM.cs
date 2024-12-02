using Ab108Uniqlo.ViewModels.Products;
using Ab108Uniqlo.ViewModels.Sliders;

namespace Ab108Uniqlo.ViewModels.Commons
{
    public class HomeVM
    {
        public ICollection<SliderListItemVM> Sliders { get; set; }
        public ICollection<ProductListItemVM> Products { get; set; }
    }
}
