namespace Ab108Uniqlo.Models;

public class ProductImage : BaseEntity
{
    public int Id { get; set; }
    public Product Products { get; set; }
    public string ImageUrl { get; set; }
}
