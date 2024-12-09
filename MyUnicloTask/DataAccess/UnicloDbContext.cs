using Ab108Uniqlo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.DataAccess
{
    public class UnicloDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> ProductImage { get; set; }
        public DbSet<ProductRating> ProductRatings { get; set; }
        public UnicloDbContext(DbContextOptions opt) : base(opt) { }
    }
}
