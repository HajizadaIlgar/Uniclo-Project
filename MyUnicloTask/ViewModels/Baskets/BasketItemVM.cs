namespace Ab108Uniqlo.ViewModels.Baskets
{
    public class BasketItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Discount { get; set; }
        public int Count { get; set; }
    }
}
