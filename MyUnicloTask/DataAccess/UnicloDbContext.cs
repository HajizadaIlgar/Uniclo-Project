using Ab108Uniqlo.Models;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.DataAccess
{
    public class UnicloDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public UnicloDbContext(DbContextOptions opt) : base(opt)
        {
        }
    }
}
