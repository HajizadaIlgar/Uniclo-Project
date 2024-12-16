using Ab108Uniqlo.Models;

namespace Ab108Uniqlo.ViewModels.Currencies
{
    public class CurrencyCreateVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
