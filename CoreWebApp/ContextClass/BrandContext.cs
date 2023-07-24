using CoreWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreWebApp.ContextClass
{
    public class BrandContext : DbContext
    {
        public BrandContext(DbContextOptions<BrandContext> options) :base(options) 
        {
                
        }
        public DbSet<Brand> brands { get; set; }
    }
}
