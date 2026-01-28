using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using SOL.MS.Security.Domain.Entities;
using SOL.MS.Security.Infraestructure.Persistence.Configurations;

namespace SOL.MS.Security.Infraestructure.Persistence
{
    public class AppDbContext : DbContext
    {
        //- Add-Migration InitialCreate -Context AppDbContext -Verbose
        //- Update-Database -Verbose -Context AppDbContext
        //- Remove-Migration -Context AppDbContext


        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());
        }
    }
}
