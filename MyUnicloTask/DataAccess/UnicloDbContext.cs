using Ab108Uniqlo.Models;
using Microsoft.EntityFrameworkCore;

namespace Ab108Uniqlo.DataAccess
{
    public class UnicloDbContext : DbContext
    {
        public DbSet<Slider> sliders { get; set; }

        public UnicloDbContext(DbContextOptions opt) : base(opt)
        {

        }
    }
}
