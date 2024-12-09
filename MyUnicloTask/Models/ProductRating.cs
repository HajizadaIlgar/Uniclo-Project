using System.ComponentModel.DataAnnotations;

namespace Ab108Uniqlo.Models
{
    public class ProductRating
    {
        public int Id { get; set; }
        [Range(1, 5)]
        public int RatingRate { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
    }
}
