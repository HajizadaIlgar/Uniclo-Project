namespace Ab108Uniqlo.Models;

public class ProductComment : BaseEntity
{
    public AppUser? User { get; set; }
    public string? UserId { get; set; }
    public Product? Product { get; set; }
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
}
