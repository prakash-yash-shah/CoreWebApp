using CoreWebApp.Models;
using JsonWebTokens.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreWebApp.ContextClass
{
    public class BrandContext : DbContext
    {
        public BrandContext(DbContextOptions<BrandContext> options) :base(options) 
        {
                
        }
        public DbSet<Brand> brands { get; set; }
        public DbSet<Tokens> tokens { get; set; }
        public DbSet<Users> users { get; set; }
        public DbSet<UserLogin> userLoginModels { get; set; }

    }
}
