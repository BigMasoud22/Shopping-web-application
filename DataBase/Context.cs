using DataBase.Fluent_Api;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataBase
{
    public class Context : IdentityDbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        #region DbSets of database
        public DbSet<Product> products { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<ShoppingCartItem> carts { get; set; }
        public DbSet<ImagesInformation> images { get; set; }
        public DbSet<UserAddress> address { get; set; }
        public DbSet<Comments> comments { get; set; }
        #endregion
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;DataBase=Shop;Trusted_Connection=True;Encrypt=False;");
            optionsBuilder.EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Fluent-API configurations
            builder.ApplyConfiguration(new ProductConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new OrderConfiguration());
            builder.ApplyConfiguration(new ShoppingCartConfiguration());
            builder.ApplyConfiguration(new ImageConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            #endregion
        }
    }
}
